using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories
{
    public class WriteRepository<T> : IWriteRepository<T> where T : BaseEntity
    {
        readonly private ETicaretAPIDbContext _context;

        public WriteRepository(ETicaretAPIDbContext context)
        {
            _context = context;
        }

        public DbSet<T> Table => _context.Set<T>();

        public async Task<bool> AddAsync(T model)
        {
            // ekleme işlemi burada gerçekleşiyor. = ekleme durumunu kontrol edip geri döndürmek için EntityEntry<T> dan değişken oluşturuyoruz
            EntityEntry<T> entityEntry = await Table.AddAsync(model); 
            return entityEntry.State == EntityState.Added; //ekleme işlemi gerçekleştirmi kontrolü yapıp geri dönüş sağlıyoruz. == yapılar entityframowrk yapıları = hazır yapıları kullanıyoruz.
        }

        // koleksiyon geldiğinde veritabanına işleme
        public async Task<bool> AddRangeAsync(List<T> datas)
        {
            await Table.AddRangeAsync(datas);
            return true;
        }

        //direk gelen veriyi sil
        public bool Remove(T model)
        {
            EntityEntry<T> entityEntry = Table.Remove(model);
            return entityEntry.State == EntityState.Deleted;
        }
        public bool RemoveRange(List<T> datas)
        {
            Table.RemoveRange(datas);
            return true;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            // buradan silinecek veriyi bulduk id ye göre
            T model = await Table.FirstOrDefaultAsync(data => data.Id == Guid.Parse(id));
            return Remove(model); //id ye göre gelen veriyi sil.
        }

        //normalde entity framworkde veri çekilir üzerinde ilgili yerlerde değişiklik yapılır ve ilgili veri güncellenir ama bu method = veriyi çekmeden direk ilgili id de şu alanları değiş diye vermek için yazıldı. = entity framwork bize bunuda sunuyor. == yani track edilmeyen veriyi güncelleme. : track mantığı var notlarda.
        public bool Update(T model)
        {
            EntityEntry<T> entityEntry = Table.Update(model);
            return entityEntry.State == EntityState.Modified;
        }

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

    }
}
