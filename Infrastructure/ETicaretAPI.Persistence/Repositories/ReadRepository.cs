using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories
{ // predicate == yüklem
    public class ReadRepository<T> : IReadRepository<T> where T : BaseEntity //genel okuma işlemleri = sadece in tabloya göre değil. = generic
    {
        private readonly ETicaretAPIDbContext _context; // ETicaretAPIDbContext.cs yi  ıos containera vermiştim bu class'ıda Ios containera verecem o sayede bu değişken direk ordan miras almış olacak. : IOS'ı container kendi içinde miras işlemlerini kontrol ediyor. = böylece classının nesnesini oluşturmama gerke kalmıyor.
                                                        // ETicaretAPIDbContext = veritabanı ile entityframworkcore üzerinden bağlantı kurduğumuz sınıf
        public ReadRepository(ETicaretAPIDbContext context) // Constructerdan injecte edebiliyorum contextimi
        {
            _context = context;
        }

        //burada _context = ETicaretAPIDbContext geliyor o class da miras olarak : DbContext : entityframwork'un class'ı alıyor  yani = _context.Set diyerek entityframwork class'ından çekiyoruz ilgili methodu. 
        public DbSet<T>  Table => _context.Set<T>();// biz tablo olarak bi T generic = entitie vermemiz lazım = customer , product diye belirli bir entities vermek istmeiyoruz : genel yapı laızm = bu yüzden Entitiy framwork core da == Set<entity>() yapısı var bizde bunu kullanıyoruz : bizim entity'imizde = T
        //Repostiyorden geldi bu evrensel

        //veritabanında T ye uygun ne kadar veri varsa getir. (IQueryable = veritabanından filtreliyerek veri getirmek içindi.)
        public IQueryable<T> GetAll(bool tracking = true)
        {
            var query = Table.AsQueryable();

            //tracking false ise == EF Core tracking'i durduruyorum projede optimizasyon yapmak için == veri okuma işlemi yaptığım için bu sayfada gerek yok EF Core tracking'e ;; default olarak true geliyor çağrırken değiştirmediğimiz sürece = o yüzden default olarak takip ediyor.
            if (!tracking)  
                query = query.AsNoTracking();
            return query;
        }

        //şarta uygun verileri getir
        public IQueryable<T> getWhere(Expression<Func<T, bool>> method, bool tracking = true)
        {
            var query = Table.Where(method);
            if(!tracking)
                query = query.AsNoTracking();
            return query;
        }

        //tek bir veri döndürmek için. == asenkron olduğu için await async mantıklarını uyguluyoruz.
        public async Task<T> GetSingleAsync(Expression<Func<T,bool>> method, bool tracking = true)
        {
            var query = Table.AsQueryable();
            if(!tracking)
                query= query.AsNoTracking();
            return await query.FirstOrDefaultAsync(method);
        }

        //=> Table.FirstOrDefaultAsync(data => data.Id == Guid.Parse(id)); // where T :class yerine where T :BaseEntity çevirme mantığını bu method için yaptık. id ye erişim için. Guid.Parse(id) ; id yi guid id ye çevirme.
        //=> await Table.FindAsync(Guid.Parse(id)); // yukarıdaki yönteme ek olarak FindAsync kullanılabilir : daha kolay.
        public async Task<T> GetByIdAsync(string id, bool tracking = true)
        {
            var query = Table.AsQueryable();
            if(!tracking)
                query= query.AsNoTracking();
            return await query.FirstOrDefaultAsync(data => data.Id == Guid.Parse(id)); //query üzerinden çalışırken FindAsync ulaşamadım ondan Mark/işaretleyici yöntemi ile id ye erişim sağlıyacam
        }
    }
}
