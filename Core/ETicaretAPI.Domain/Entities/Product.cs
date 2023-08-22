using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities
{
    public class Product : BaseEntity //entityler domain katmanında mimari teorsinde anlatıldığı gibi
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public float Price { get; set; }
        //public ICollection<Order> Orders { get; set; } //Productın birden çok orderı olabiliyor. == mantığı order sayfasında entites de açıkladım.
        public ICollection<ProductImageFile> ProductImageFiles { get; set; } //bir ürününn birden fazla resmi olabilir.
        public ICollection<BasketItem> BasketItems { get; set; }
    }
}
