using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Persistence.Contexts //bu classın amacı sql deki tabloların proğram içindeki karşılığı ?
{
    public class ETicaretAPIDbContext : IdentityDbContext<AppUser, AppRole, string> //DbContext // mirası Microsoft.EntityFrameworkCore'dan alıyoruz : ad önemli değil miras almamız önemli 
    {
        public ETicaretAPIDbContext(DbContextOptions options) : base(options) { } //ctrl+. diyerek bu contractırı oluşturuyorum IOC Containerda doldurulacak : bu contstractırı oluşturmaz sam süreçte hata alırım.

        // burada bizim oluşturduğumuz entitylere karşı : Product,Order,Customer karşılık = Products, Orders,Customers tablolarını oluştur diyorum : entity propertylerinide tablo özelliği yap. bunu DbSet ile belirtiyorum : tabloları oluştur diye

        public DbSet<Product> Products { get; set; } //tablo isimleri defaul olarak burda belirttiğim Products oluyor mesela biz bunu sonradan müdahale ile değiştirebiliyoruz isteğe göre.
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Domain.Entities.File> Files { get; set; } //system.io.files ile çakıştığından tam yolu ile veriyoruz : burdaki dir diye.
        public DbSet<ProductImageFile> ProductImageFiles { get; set; }
        public DbSet<InvoiceFile> InvoiceFiles { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Order>()
                .HasKey(o => o.Id);

            builder.Entity<Basket>()
                .HasOne(b => b.Order)
                .WithOne(o => o.Basket)
                .HasForeignKey<Order>(o => o.Id);

            // IdentityDbContext i kullandığımız için dbcontext yerine butür override larda base vermek gerekiyor.
            base.OnModelCreating(builder);
        }


        // burada ben değer verilen entityleride değiştirebilirim : verilmeyenlerede atama yapabilirim datalar verilip = güncelleme veya ekleme : savechanges geldikten sonra tetiklenmir burası o yüzden o datalara erişebiliyorum aşşağıda da eriştim zaten.
        //Burası sayesinde her SaveChanges tetiklendiğinde araya girip istediğim controlleri sağlayacam = interceptor == entityframwork de uyguluyoruz
        //SaveChanges'in 4 tane methodu vardı buraya ovveride ederken ben default paremetre alanı seçiyorum çünkü writeRepository de onu kullandım.
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var datas = ChangeTracker
             .Entries<BaseEntity>(); // bu yapı sayesinde datas ' a BaseEntity'den gelen verileri yakalıyoruz. BaseEntity de = ortak kullandığımız her tabloda olması gereken datalar
               // Entityler üzerinden yapılan değişiklerin ya da  yeni eklenen verinin yakalanamasını sağlayan propertydir. Update operasyonlarında Track edilen verileri yakalayıp elde etmemizi sağlar. == tabi track : insert edilen veriyi yakalıyamaz çünkü insert sırasında track kullanılmaz = veritabanından okumalarda track yapısı kullanılıyordu değişikliğ algılamak için.

            foreach (var data in datas)
            {
                //ilk şartı kontrol eder ilk şartsa ilke girer, ilk şart değilse = ikinci şartı kontrol eder : ikinci şartsa ikinci şarta girer. == ikinci şart'da değilse son şarta girer yani 2. şarta girer yine.
                // if yerine kullandık burada == data.State EntityState.Added ise  = EntityState.Added => data.Entity.CreatedDate = DateTime.UtcNow bu işlemi yap ; data.State  = EntityState.Modified buysada  data.Entity.UpdatedDate = DateTime.UtcNow işlemini yap. gibisinden == if 'in farklı bi yöntemi
                _ = data.State switch 
                {
                    EntityState.Added => data.Entity.CreatedDate = DateTime.UtcNow, //ekleme işlemlerinde CreatedDate verilen datayı ata
                    EntityState.Modified => data.Entity.UpdatedDate = DateTime.UtcNow, // güncelleme işlemlerinde UpdatedDate verilen datayı ata.
                    _ => DateTime.UtcNow // silme işleminde silinmiş verinin tarihinde işlem yapamıyacağından bu şartı koyuyoruz.
                }; //EntityState.Added ; EntityState.Modified kullanımları geriye dönüş sağlıyor biz dönüş ile ilgilenmediğimizden _ = data.State switch dedik geri dönüşü olmasaydı ==  data.State switch demem yeterliydi büyük ihtimal.
            }

            return await base.SaveChangesAsync(cancellationToken); // methodda task olduğu için meyhoda = async ; buraya await eklemeyi unutmuyoruz
            // SaveChangesAsync zaten default olarak çalışan o yüzden ben üstte işlemimi yapıp bunun çalışmasına devam etmesine izin verecem.
        }
    }
}
