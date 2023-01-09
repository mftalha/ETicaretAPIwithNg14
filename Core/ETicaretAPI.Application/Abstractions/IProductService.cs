using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions //Abstraction = soyut
{
    public interface IProductService //biz burda anahtar interface oluşturuyoruz : sadece iskelet için : Abstractions klasöründe = application katmanında 
    {
        List<Product> GetProducts();
    }
}
