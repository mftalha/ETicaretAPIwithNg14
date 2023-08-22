using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities
{
    public class Order : BaseEntity
    {
        //public Guid CustomerId { get; set; } // entity framwork yapısal olarak : customer burda tekil olduğu için 1- n de : 1 kısmı olduğu için ona Id atıyacak veritabanı alanında otomatik. biz burda eğer bu isimle bunu vermesek zaten kendi veritabanı kısmında yine oluşturur ama biz veritabanındaki değere karşılık bu gelsin diye belirtmek için kendimiz oluşturuyoruz. ismi farklı yapabiliriz. ama o zaman belirmemiz gerekiyormus : belli şekillerde.
        public string Description { get; set; }
        public string Address { get; set; }

        // n mantıklarında = çoktan 1 olana entity'e bir ICollection verilir
        //public ICollection<Product> Products { get; set; } //bu orderın prodoct ile n ilişkisi olduğunu gösterir : böyle bırakırsak product'a bişey yazmassak : bir order'ın birden çok product'ı olduğunu ifade eder. ama biz prodoct'ada aynı şekil belirterek n e n ilişki olduğunu belirtecez. == (yani Icollectionu koyduğumuz sayfadaki entity 1 : i , Icollection ile belirttiğimiz entities ise n ilişkisini ifade eder (n : birden fazla olabilir anlamında))
        //public Customer customer { get; set; }
        public Basket Basket { get; set; }

    }
}
