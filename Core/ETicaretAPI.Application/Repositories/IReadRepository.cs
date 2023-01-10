using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories
{
    // where T: class burada gelecek T class olacak diye belirtme sebeim IRepository'e de gönderilecek olması bu IReadRepository 'e gelen T nin ve IRepository de DbSet den dolayı T yi class istiyordu.
    // IReadRepository<T> diye belirtiyoruz. böylece burdan gelen T yi IRepository<T> ye gönder demiş oluyoruz
    //veritabanından okuma yapma fonskiyonları burada tanımlanacak.
    //Read'dan kasıt : sadece select işlemleridir. = okuma sadece.
    public interface IReadRepository<T> : IRepository<T> where T : BaseEntity
    {
        // - IQueryable: Sorgu üzerinde çalışmak istiyorsak : yani verdiğimiz sorgulara göre veritabanından veriler gelir ; - IEnumerable = bütün verileri veritabanından çekersin ve çektiğin veriler üzerinden işlem yaparsın. == IQueryable kullanmak doğrusu.
        IQueryable<T> GetAll(bool tracking = true);  // tracking işlemini gerekmediği yerlerde durdurmak için = bool tracking = true ==> paremetresini ekliyorum. // tracking : veritabanından veri çekildiğinde arka planda .net in o verileri tutması : güncelleme işlemlerinde ordaki veri ile karşılaştırıp veritabanına farklılık varsa o farkları yazma gibi mantık var : sadece okuma yaparken bu takibe gerek olmadığından gerekli durumlarda durdurmak için böyle bi paremetre veriyoruz.
        //hangi türde isek (<T> ile anlıyacaz türü) : bütün verileri getir bana.
        IQueryable<T> getWhere(Expression<Func<T, bool>> method, bool tracking = true); // where şartı mahiyetinde kullanacaz.
        // method yerine ahmet de yazabilirim.
        // bi şart verecem : şarta uygun birden fazla veri elde edecem.
        // (<Fun)bu özel fonksiyona , (<T, bool>)gelen verilerden true olanları döndür
        Task<T> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = true);
        // arka planda asenkron fonksiyon kullanacak bu yüzden method ismi sonuna Async ekliyorum: çünkü asenkron çalılacak fonksiyonlar Async keyword'u ile biterler isimsel olarak.
        // başındada T yi , Task<T>  : yaparak veriyi geriye Task olarak döndürüyorum.
        // verilen şartı sağlayan ilk veriyi getirecek Ondan T yazdık başa IQueryable yazmadık 
        Task<T> GetByIdAsync(string id, bool tracking = true);
        // GetSingleAsync de açıkladım asenkron mantığını aynı.
        // id uygun olan hangisiyse onu getirecek
        // Not: yukarıdaki işlemleri yapması için ek kodlama işlemi yapacaz anladığım kadarıyla.

    }
}
