using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace ETicaretAPI.API.Extensions;

// Burası daha geliştirilebilir => işe yarar seviyede bıraktık.
// Uygulamanın herhangi bir yerinde patlama olduğunda buraya düşecek
// Extension fonksiyon barındıracağından static olarak tanımlıyoruz class'ı
static public class ConfigureExceptionHandlerExtension
{
    //program.cs içinde app.ConfigureExceptionHandler(); şeklinde kullanabilmek için => this WebApplication application paremetre alıyoruz çünkü program.cs içindeki app referansı WebApplication dan referans alır
    public static void ConfigureExceptionHandler<T>(this WebApplication application, ILogger<T> logger)
    {
        application.UseExceptionHandler(builder =>
        {
            //uygulamanın herhangi bir yerinde patlama yaşandığında bunu işlememizi sağlıyacak
            builder.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType =  MediaTypeNames.Application.Json; // Geri dönüş type belirtirken bu şekilde güvenli belirtme yapabiliyoruz: diğer türlü "application/json" gibi yazmalıyız ve hata lı yazma gibi durumlar meydana gelebilir.

                // hata ile ilgili çoğu bilgiyi getirecek bize
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    //loglama => burda özelleştirme yapılabilir mesajlar hakkında.
                    logger.LogError(contextFeature.Error.Message);

                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = contextFeature.Error.Message,
                        Title = "Hata Alındı!"
                    }));
                }
            });
        });
    }
}
