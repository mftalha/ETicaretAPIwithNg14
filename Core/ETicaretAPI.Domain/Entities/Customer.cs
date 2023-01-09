using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<Order> Orders { get; set; } //bir costamerin birden fazla orderı olabilir ama bir ordar'ın birden fazla customr'ı olamaz bu yüzden (customer - order ilişkisi = 1 - n) bu yüzden customer entities'i i içinde n Order seçebildiğimizden Icollection ile tanımlıyoruz.  Ama Order içinde bir Custormer seçebildiğimiz için : Order içinde tekil Customer tanımlıyacaz.
    }
}
