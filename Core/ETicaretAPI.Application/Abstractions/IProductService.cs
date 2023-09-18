using ETicaretAPI.Domain.Entities;

namespace ETicaretAPI.Application.Abstractions; //Abstraction = soyut

public interface IProductService //biz burda anahtar interface oluşturuyoruz : sadece iskelet için : Abstractions klasöründe = application katmanında 
{
    List<Product> GetProducts();
}
