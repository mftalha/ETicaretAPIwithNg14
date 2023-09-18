﻿using ETicaretAPI.API.Configurations.ColumnWriters;
using ETicaretAPI.API.Extensions;
using ETicaretAPI.API.Filters;
using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infrastructure;
using ETicaretAPI.Infrastructure.Filters;
using ETicaretAPI.Infrastructure.Services.Storage.Azure;
using ETicaretAPI.Persistence;
using ETicaretAPI.SignalR;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region test
// alışveriş sepeti altyapısı - 52. ders
// NameClaimType = ClaimTypes.Name => loglama için olusturulan bu veriye erişmek için oluşturuldu.
// clientten gelen request neticesinde oluşturulan httpcontext nesnesine katmanlardaki classlar üzerinden(business logic) erişebilmemizi sağlayan bir sevistir.
builder.Services.AddHttpContextAccessor(); 
#endregion

//Apý katmanýndan Persistence katmanýna eriþim için yazdýðýmýz methodu depending enjectiona enjecte ediyoruz. = tabi bunun için API katmanýna Persistence katmanýný referans olarak veriyoruz.
builder.Services.AddPersistenceServices();
//Apý katmanýndan Infrastructure katmanýna eriþim için yazdýðýmýz methodu depending enjectiona enjecte ediyoruz. =  bunun için API katmanýna Infrastructure katmanýný referans olarak veriyoruz.
builder.Services.AddInfrastructureServices();
//builder.Services.AddStorage(StorageType.Azure); // paremetreye ne verirsem onun ile ilgili depolama iþkeminbi yapsýn : azure storage , aws , local storage 
//builder.Services.AddStorage<LocalStorage>(); //Mimari artýk storage iþlemlerini : LocalStorageye göre yapacaktýr.

builder.Services.AddApplicationServices();
builder.Services.AddSignalRServices();

builder.Services.AddStorage<AzureStorage>(); 

//cors politikalarý için servis oluþturma => app.UseCors(); diye aþþagýdan eklemeyi unutmamalýyým
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials()
// AllowCredentials() => SignalR isteklerinede izin veriyoruz.
//bu adresten gelen istekleri kabul et =  cros politikasý için yazýldý. == bu adres dýþýndan gelen istekler eriþemez.
//policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin() //böyle dediðimde her gelen istek atabilecek belirli bir url sýnýrý koymadýk daha.
)); //2 seçenek var sitelere göre veya default olarak projeye göre


#region Serilog - Postgresql
//UseSerilog'u çağırdığımızda builderdaki log mekanizması ile değiştirmiş oluyoruz

//Loger =>  using Serilog.Core; dan gelmeli
Logger log = new LoggerConfiguration()
    .WriteTo.Console() // Consola loglama yap
    .WriteTo.File("logs/log.txt") // logs/log.txt dosyasını yoksa oluştur buraya loglama yap
                                  //PostgreSQL(connection string, tablo ismi, tabloyu bizmi oluşturacak yoksa otomatikmi oluşturulacak : otomatik seçiyoruz, kendi tablo default alanları hariç alanalr oluşturmak istiyorsak tabloda.)
    .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostgreSQL"), "logs", needAutoCreateTable: true, columnOptions: new Dictionary<string, ColumnWriterBase> // ekstra tablo alanları : defaultlara ek
    {
        {"message", new RenderedMessageColumnWriter() }, // RenderedMessageColumnWriter isimli nesnenin elde ettiği veri message kolonuna yazdırılacak => message ismini farklı verebilirim ne istersem.
        {"message_template", new MessageTemplateColumnWriter() },
        {"level", new LevelColumnWriter() },
        {"time_stamp", new TimestampColumnWriter() },
        {"exception", new ExceptionColumnWriter() },
        {"log_event", new LogEventSerializedColumnWriter() },
        {"user_name", new UsernameColumnWriter() }
    }) // PostgreSQL'e loglama yap.
    .WriteTo.Seq(builder.Configuration["Seq:ServerURL"]) // Seq üzerinden loglamayı takip edebilmek için.
    .Enrich.FromLogContext() // harici bir beslenme varsa  => context'ten user_name alma gibi bunu eklemeliyiz.
    .MinimumLevel.Information() // hangi seviyeden sonrasının loglarını tutmalıyız : warning, error'da olabilir.
    .CreateLogger();
builder.Host.UseSerilog(log);

#region Uygulamada yapılan requestleride log sayesinde yakalıyabilmek için

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua"); // kullanıcıya dair tüm bilgileri getirir
    //logging.ResponseHeaders.Add("MyResponseHeader"); // respose header'a key ekliyebiliriz.
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

#endregion

#endregion




//controllar mekanizmasýnýn bildiðimiz gibi çalýþmasýný saðlayan servis.
//builder.Services.AddControllers()
/*
builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>()) //paremetre olarak  verdiðim yapý sayesinde artýk : validationlarý ben kontrol edecem ve sonuçlara göre istediðim gibi dönüþ saðlayabileceðim. ValidationFilter servisi ile.
*/
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<RolePermissionFilter>(); // araya girmek için buraya eklendi.
}) //paremetre olarak  verdiðim yapý sayesinde artýk : validationlarý ben kontrol edecem ve sonuçlara göre istediðim gibi dönüþ saðlayabileceðim. ValidationFilter servisi ile.
    .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>()) // AddFluentValidation : validation iþlemleri için kullandýðýmýz kütüphaneydi : nuget'ten indirip projemize dahil etmiþtik application katmanýnda. ilgili validationlarý : ilgili methodlara baðladýðýmýzý artýk veri iþlemlerinde validationlarýda kontrol etmesi gerektiðini projeye burada veriyoruz. - RegisterValidatorsFromAssemblyContaining methodunu kullanma sebimiz ise her oluþturduðumuz validation modelini burda teker teker tanýmlamamak için CreateProductValidator diye 1 tane validationun yolunu veriyoruz : ve artýk bundan sonra diðer validationlarýda kendisi o dosya yolundan alacaktýr. : bizim teker teker her seferinde vermemize gerek yok artýk.
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true); // burasý sayesinde mvc nin gelen veriler üzerinden filtrelemeye uymayan yapýlarý otomatik yakalamasýný kapattýk. : biz yakalýyacaz controlalrda : þartlar ile.


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region jwt doğrulama
// AddAuthentication(JwtBearerDefaults.AuthenticationScheme) : default olarak shema alıyoru bu
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	////"Admin" : schema ismim => Authentication schema yaptık
	.AddJwtBearer("Admin", options => // bu uygulamaya token geliyorsa bu token'ı doğrularken jwt olduğunu bil
    {   // Bu jwt'yi doğrularken burdaki konfigurasyonlar üzerinden doğrula 
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true, //(kitle)Oluşturulacak token değerini kimlerin/hangi originlerin/sitelerin kullanacığını belirlediğimiz değerdir. => www. ... .com
            ValidateIssuer = true, //(ihraççı) Oluşturulacak token değerini kimin dağıttını ifade edeceğimiz alandır. => www.myapi.com
            ValidateLifetime = true, //Oluşturulan token değerininin süresini kontrol edecek doğrulamdır. : süresi geçtimi geçmedimi ?
            ValidateIssuerSigningKey = true, // Üretilecek token değerinin uygulamamıza ait olduğunu ifade eden seciry key verisinin doğrulanmasıdır. => üsttekiler tahmin edilebilir olsada bu değeri çok farklı koyup token değerinin değerini daha fazla tahmin edilemez yapıp token güvenliğini sağlıyabiliriz.

            // hangi değerler ile doğrulşaması sağlanacak yukarıdaki talep edilenlerin cevabı.
            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            // apiye gelen isteklerde token'ın expire olup olmadığını kontrol ediyoruz(süresi dolmuşmu)
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow: false,
            NameClaimType = ClaimTypes.Name // JWT üzerinde Name Claimne karşılık gelen değeri User.Identity.Name properysinden elde edebiliriz. => loglama için eklendi bu satır. => loglamada name elde etmek için.
		};
    });

#endregion

var app = builder.Build();
// Middleware işelmlerini WebApplication'a sahip app referansın üzerinden çağırabiliyoruz.

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// uygulamanın herhangi bir yerinde patlama olduğunda bunun üzerinden kontrol edebiliriz.
//app.UseExceptionHandler();
// app.Services.GetRequiredService<ILogger<Program>>() => buna karşılık olan Logger'ı bize getirecektir.
app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>()); //program.cs kalabalıklaşmasın diye extension üzerinden işlemleri yapıp extension'ı burada çağırıyruz => Middleware işlemelrinde böyle yapmak daha doğru.


app.UseStaticFiles(); // api içinde new folder diyip : ismine wwwroot dediðimde : direk dünya simgeli bir dosya oluþacak : bu özel bi dizin ve bunun için program.cs de bu kýsmý tanýmlamam gerekli. == tarayýcýdan url kýsmýndan bu klasörün içindkei dosyalara eriþilemez sadece kod içinden eriþilebiliyor : statik olarak tutuluyorlar : sunucuda tutuluyor dosyalarýmýz güvenli bir þekilde : wwwroot içinde tutuyoruz.. 

//yukarýda belirlediðim cors politikasýnýný çaðoýrýyoruz.

#region Serilog - Postgresql 
// serilog'un çalışması için çağırıyoruz.
// kendisinden önceki middlewares çalışyırılmıyor ondan en üste koymalıyız. alttakikerin loglanması için.
app.UseSerilogRequestLogging();
#region Uygulamada yapılan requestleride log sayesinde yakalıyabilmek için
app.UseHttpLogging();
#endregion
#endregion

app.UseCors();
//

app.UseHttpsRedirection();

#region jwt 
app.UseAuthentication();
#endregion
app.UseAuthorization();

#region Serilog - Postgresql 
// bu middleware işlemlerinide extension'a almam daha doğru : yapmalıyım bi boşlukta.
// : usernameyi almak için araya girmek(middleware) için => app.UseAuthentication(), app.UseAuthorization() işlemlerinden  sonra olmalı username authentication işleminden sonra alabilceğimiz için. 
app.Use(async (context, next) => // context o anki request, next sonrasındaki işlemler için.
{
    // loglamada user_name değerini tabloya kaydetmek istediğmizden username alanını elde etmek için bu işlem.
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
    LogContext.PushProperty("user_name", username);
    await next();
});
#endregion

app.MapControllers();
#region SignalR 
// Extension
app.MapHubs();
#endregion


app.Run();
