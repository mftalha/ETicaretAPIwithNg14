using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.ViewModels.Product
{
    public class VM_Create_Product // view model oluşturma sebimiz : dış dünyadan gelecek istekler için model = direk entity ile bağlantı kurmasın diye. dış dünyadan gelen istek.
                                    // wiew model geçici süreliğine oluşturuldu. ilerde farklı bir yöntem kullanacaz. 
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public float Price { get; set; }
    }
}
