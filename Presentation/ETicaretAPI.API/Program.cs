using ETicaretAPI.Persistence;

var builder = WebApplication.CreateBuilder(args);

//Ap� katman�ndan Persistence katman�na eri�im i�in yazd���m�z methodu depending enjectiona enjecte ediyoruz. = tabi bunun i�in API katman�na Persistence katman�n� referans olarak veriyoruz.
builder.Services.AddPersistenceService();

// Add services to the container.

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
