using ETicaretAPI.Persistence;

var builder = WebApplication.CreateBuilder(args);

//Apý katmanýndan Persistence katmanýna eriþim için yazdýðýmýz methodu depending enjectiona enjecte ediyoruz. = tabi bunun için API katmanýna Persistence katmanýný referans olarak veriyoruz.
builder.Services.AddPersistenceService();

//cors politikalarý için servis oluþturma => app.UseCors(); diye aþþagýdan eklemeyi unutmamalýyým
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod()
//bu adresten gelen istekleri kabul et =  cros politikasý için yazýldý. == bu adres dýþýndan gelen istekler eriþemez.
//policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin() //böyle dediðimde her gelen istek atabilecek belirli bir url sýnýrý koymadýk daha.
)); //2 seçenek var sitelere göre veya default olarak projeye göre

builder.Services.AddControllers();
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

//yukarýda belirlediðim cors politikasýnýný çaðoýrýyoruz.
app.UseCors();
//

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
