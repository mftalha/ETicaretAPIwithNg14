using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories
{//burada write işlemleri için methodların iskeletini oluşturuyoruz.
    public  interface IWriteRepository<T> : IRepository<T> where T : BaseEntity //veritabanına ekleme , güncelleme , silme işlemleri için oluşturuldu.
    {
        //Bool verme sebebim = ekleme sonucu true yada false dönmesini istemem.
        Task<bool> AddAsync(T model); //ekleme işleminde orm de asy gerçekleştirileceğimizden = gelen model ne ise =  tane ekleme
        Task<bool> AddRangeAsync(List<T> datas); // bir koleksiyon geldiğinde veritabanına işleme == AddAsync bir overloadını oluşturma.
        bool Remove(T model);
        bool RemoveRange(List<T> datas); // birden fazla veri silmek için 
        Task<bool> RemoveAsync(string id); // id ye göre silme için
        bool Update(T model); //güncelleme işlemi için
        Task<int> SaveAsync(); // yapılan işlemler sonunda savechange yi çağırmak için bu fonksiyonu kullanacaz.
    }
}
