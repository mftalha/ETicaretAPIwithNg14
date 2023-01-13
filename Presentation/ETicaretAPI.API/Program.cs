using ETicaretAPI.Persistence;

var builder = WebApplication.CreateBuilder(args);

//Ap� katman�ndan Persistence katman�na eri�im i�in yazd���m�z methodu depending enjectiona enjecte ediyoruz. = tabi bunun i�in API katman�na Persistence katman�n� referans olarak veriyoruz.
builder.Services.AddPersistenceService();

//cors politikalar� i�in servis olu�turma => app.UseCors(); diye a��ag�dan eklemeyi unutmamal�y�m
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod()
//bu adresten gelen istekleri kabul et =  cros politikas� i�in yaz�ld�. == bu adres d���ndan gelen istekler eri�emez.
//policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin() //b�yle dedi�imde her gelen istek atabilecek belirli bir url s�n�r� koymad�k daha.
)); //2 se�enek var sitelere g�re veya default olarak projeye g�re

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

//yukar�da belirledi�im cors politikas�n�n� �a�o�r�yoruz.
app.UseCors();
//

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
