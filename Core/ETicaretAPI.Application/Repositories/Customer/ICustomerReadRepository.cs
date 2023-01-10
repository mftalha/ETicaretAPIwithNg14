using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace ETicaretAPI.Application.Repositories.Customer // şeklindeyken .Customer'ı sildim = çünkü  <Customer> 'ı çağırmama izin vermiyor diğer türlü ondan Customer dosyası altında tutsamda namespace de Repositories altında kalması şeklinde ayarladık : hatayı gidermek için.
namespace ETicaretAPI.Application.Repositories
{
    public interface ICustomerReadRepository : IReadRepository<Customer> // artık burda Customer'a özel  IReadRepository fonskyionlarını barındıracak.
    {
    }
}
