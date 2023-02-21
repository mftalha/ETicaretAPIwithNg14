using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities
{
    //burda file'dan türetme sebebim : Entity framwork Table Per Hierarchy mimarisini kullanmak için.
    public class ProductImageFile : File 
    {
        public ICollection<Product> Products { get; set; }    
    }
}
