using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infrastructure;
using ETicaretAPI.Infrastructure.Filters;
using ETicaretAPI.Infrastructure.Services.Storage.Azure;
using ETicaretAPI.Persistence;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Apý katmanýndan Persistence katmanýna eriþim için yazdýðýmýz methodu depending enjectiona enjecte ediyoruz. = tabi bunun için API katmanýna Persistence katmanýný referans olarak veriyoruz.
builder.Services.AddPersistenceServices();
//Apý katmanýndan Infrastructure katmanýna eriþim için yazdýðýmýz methodu depending enjectiona enjecte ediyoruz. =  bunun için API katmanýna Infrastructure katmanýný referans olarak veriyoruz.
builder.Services.AddInfrastructureServices();
//builder.Services.AddStorage(StorageType.Azure); // paremetreye ne verirsem onun ile ilgili depolama iþkeminbi yapsýn : azure storage , aws , local storage 
//builder.Services.AddStorage<LocalStorage>(); //Mimari artýk storage iþlemlerini : LocalStorageye göre yapacaktýr.

builder.Services.AddApplicationServices();

builder.Services.AddStorage<AzureStorage>(); 

//cors politikalarý için servis oluþturma => app.UseCors(); diye aþþagýdan eklemeyi unutmamalýyým
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod()
//bu adresten gelen istekleri kabul et =  cros politikasý için yazýldý. == bu adres dýþýndan gelen istekler eriþemez.
//policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin() //böyle dediðimde her gelen istek atabilecek belirli bir url sýnýrý koymadýk daha.
)); //2 seçenek var sitelere göre veya default olarak projeye göre



//controllar mekanizmasýnýn bildiðimiz gibi çalýþmasýný saðlayan servis.
//builder.Services.AddControllers()
builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>()) //paremetre olarak  verdiðim yapý sayesinde artýk : validationlarý ben kontrol edecem ve sonuçlara göre istediðim gibi dönüþ saðlayabileceðim. ValidationFilter servisi ile.
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
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow: false 
		};
    });

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // api içinde new folder diyip : ismine wwwroot dediðimde : direk dünya simgeli bir dosya oluþacak : bu özel bi dizin ve bunun için program.cs de bu kýsmý tanýmlamam gerekli. == tarayýcýdan url kýsmýndan bu klasörün içindkei dosyalara eriþilemez sadece kod içinden eriþilebiliyor : statik olarak tutuluyorlar : sunucuda tutuluyor dosyalarýmýz güvenli bir þekilde : wwwroot içinde tutuyoruz.. 

//yukarýda belirlediðim cors politikasýnýný çaðoýrýyoruz.
app.UseCors();
//

app.UseHttpsRedirection();

#region jwt 
app.UseAuthentication();
#endregion
app.UseAuthorization();

app.MapControllers();

app.Run();
