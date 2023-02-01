using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infrastructure.Filters;
using ETicaretAPI.Persistence;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//Ap� katman�ndan Persistence katman�na eri�im i�in yazd���m�z methodu depending enjectiona enjecte ediyoruz. = tabi bunun i�in API katman�na Persistence katman�n� referans olarak veriyoruz.
builder.Services.AddPersistenceService();

//cors politikalar� i�in servis olu�turma => app.UseCors(); diye a��ag�dan eklemeyi unutmamal�y�m
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod()
//bu adresten gelen istekleri kabul et =  cros politikas� i�in yaz�ld�. == bu adres d���ndan gelen istekler eri�emez.
//policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin() //b�yle dedi�imde her gelen istek atabilecek belirli bir url s�n�r� koymad�k daha.
)); //2 se�enek var sitelere g�re veya default olarak projeye g�re



//controllar mekanizmas�n�n bildi�imiz gibi �al��mas�n� sa�layan servis.
//builder.Services.AddControllers()
builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>()) //paremetre olarak  verdi�im yap� sayesinde art�k : validationlar� ben kontrol edecem ve sonu�lara g�re istedi�im gibi d�n�� sa�layabilece�im. ValidationFilter servisi ile.
    .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>()) // AddFluentValidation : validation i�lemleri i�in kulland���m�z k�t�phaneydi : nuget'ten indirip projemize dahil etmi�tik application katman�nda. ilgili validationlar� : ilgili methodlara ba�lad���m�z� art�k veri i�lemlerinde validationlar�da kontrol etmesi gerekti�ini projeye burada veriyoruz. - RegisterValidatorsFromAssemblyContaining methodunu kullanma sebimiz ise her olu�turdu�umuz validation modelini burda teker teker tan�mlamamak i�in CreateProductValidator diye 1 tane validationun yolunu veriyoruz : ve art�k bundan sonra di�er validationlar�da kendisi o dosya yolundan alacakt�r. : bizim teker teker her seferinde vermemize gerek yok art�k.
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true); // buras� sayesinde mvc nin gelen veriler �zerinden filtrelemeye uymayan yap�lar� otomatik yakalamas�n� kapatt�k. : biz yakal�yacaz controlalrda : �artlar ile.


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//yukar�da belirledi�im cors politikas�n�n� �a�o�r�yoruz.
app.UseCors();
//

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
