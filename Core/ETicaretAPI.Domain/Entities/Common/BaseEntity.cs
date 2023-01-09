using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities.Common
{
    public class BaseEntity //bütün entity lerde kullanılacak ortak Propertyleri tanımlıyorum :  burdan çekecem diğer entitylere = tekrar tekrar yazmamak için
    { 
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set;}
    }
}
