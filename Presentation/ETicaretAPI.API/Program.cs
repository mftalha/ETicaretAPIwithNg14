using ETicaretAPI.Persistence;

var builder = WebApplication.CreateBuilder(args);

//Apý katmanýndan Persistence katmanýna eriþim için yazdýðýmýz methodu depending enjectiona enjecte ediyoruz. = tabi bunun için API katmanýna Persistence katmanýný referans olarak veriyoruz.
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
