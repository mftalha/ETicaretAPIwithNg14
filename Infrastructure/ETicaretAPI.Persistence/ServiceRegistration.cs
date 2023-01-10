using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Persistence.Contexts;
using ETicaretAPI.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence
{
    public static class ServiceRegistration
    {
        //API katmanımda program.cs içinde depending enjection ile ben bu methodu IOC Container'a ekledim o yüzden buraya ne eklersem IOC Containara eklenmiş olacak.
        public static void AddPersistenceService(this IServiceCollection services) 
        {
            //burada yaptığımız şey oluşturduğumuz dbcontext'i uygulayacağımız veritabanı bağlantısını yapıyoruz. biz postgresql kullanıyoruz bu projede o yüzden. - persistence katmanına sağ tıklayıp Manage Nuget Package ye tıklıyorum. -Browser kısmına tıklayıp Npgsql.EntityFrameworkCore.PostgreSql i aratıyorum ve indiriyorum projeye - options.UseNpgsql görülmesi için sayfsaya : using Microsoft.EntityFrameworkCore; ekliyorum üste artık options.UseNpgsql gözükecektir zaten options.Use yazdığımda devamında tıklıyorum ve içine connection stringi giriyorum.
            services.AddDbContext<ETicaretAPIDbContext>(options => options.UseNpgsql(Configuration.ConnectionString)); // default olarak scope oluşuyordu gelen hatadan dolayı ;; ServiceLifetime.Singleton a çevirdik. == tekrar scop yaptık en son hali

            //magrationların oluşturulması : -ilk olarak Package Manager Consolu açıyoruz. -Default Procekte magrationlar nerede oluşturulmasını istiyorsak onu seçiyoruz : biz veritabanı işlemi olduğu için Persistence katmanını seçiyoruz. - add-migration mig_1 yazıyoruz (mig_1 ismini verme sebebimiz farklı migration set işlemleride yapacaz yerine göre silmemiz gerekecek : o yüzden bi düzen olması için biz öyle isimlendiriyoruz.) entera basıyoruz

            // - yukarıdaki işlemin gerçekleşebilmesi için önce startup proje kaytmanında(API katmanında) yani bir nuget eklememiz gerekiyor: API katmanına sag tıklıyoruz Manage Nuger Package'yi seçiyoruz. Browse kısmından Microsoft.EntityFrameworkCore.Design 'i indiriyoruz. sonra  tekrar add-migration mig_1 çalıştırdığımda Persistence katmanımın içinde Migrations katmanının oluşmuş olmasını bekliyorum.: hata aldım projeyi rebuild yapıp tekrar deniyice çalıştı. böylede hata verise bide persistence katmanına sag tık yapıp Manage Nuget Packages seçiyoruz. ordan browser'ı seçiyoruz :  Microsoft.EntityFrameworkCore.Tools indirip projeyi tekrar rebuild ediyoruz. ve ektrar add-migration mig_1 diyoruz

            // migration oluşturmada 2. yöntem ise : migration oluşturulacak katmanın dosya yolunda (Persistence) cmd yi açıyoruz = dotnet ef migrations add mig_1 yazıp enterliyoruz - hata alacaz - çözümü için - persistence(migration oluşturulan katman) katmanına sag tık yapıp Manage Nuget Packages seçiyoruz. ordan browser'ı seçiyoruz :  Microsoft.EntityFrameworkCore.Tools indiriyoruz. - ve tekrar dotnet ef migrations add mig_1 diyoruz. yine hata alacaz. - Persistence katmanında DesignTimeDbContextFactory adında class oluşturuyoruz ve içini dolduruyoruz: ilgili sayfada devamı. - sayfa tamamlandıktan sonra cmd de yine aynı dosya yolunda dotnet ef migrations add mig_1 yazdığımda Persistence katmanında migration'umu oluşturur.

            //Not : bizim magration oluşturma sebebimiz : oluşturduğumuz entityleri veritabanının anlayacağı hale çevirmek.

            // oluşturduğum migration doğrultusunda veritabanında tablolarımı oluşturmak için package manager consol'da update-database yazıyorum ; eğerki cmd veya powershell üzerinden yapmak istersekde persistence katman yolunda : dotnet ef database update diyoruz. : bu komut sonrası migrationlardaki tabloları veritabanına gönderecektir.

            //bunları toplu eklemek için bir yöntem varmış kullanılabilir.
            //scope = bir request esnasında herhangi bir nesnenin bir örneğini oluşturur ondan döndürür = her yeni bir requestte yeni bir nesne döndürecektir. scope = kullanıyorduk kaldırdık hatadan dolayı =  services.AddScoped

            services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();
            services.AddScoped<ICustomerWriteRepository, CustomerWriteRepository>();
            services.AddScoped<IOrderReadRepository, OrderReadRepository>(); //repostitoryleri tekil ekleme sağlıyoruz   
            services.AddScoped<IOrderWriteRepository, OrderWriteRepository>();
            services.AddScoped<IProductReadRepository, ProductReadRepository>();
            services.AddScoped<IProductWriteRepository, ProductWriteRepository>();

            //context nesnesini default olarak ServiceLefetime.Scope eklediğinden Repostory servislerini aynı yaşam döngüsünde eklememizin yararı olurmuş. = ondan AddScoped da ekliyoruz.

        }
    }
}
