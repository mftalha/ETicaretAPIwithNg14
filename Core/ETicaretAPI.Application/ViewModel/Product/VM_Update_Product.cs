using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.ViewModel.Product
{
    public class VM_Update_Product
    {
        public string Id { get; set; } //id'yi paremetre olarakda alabiliriz. ama böyle gövdede almak daha doğruymuş.
        public string Name { get; set; }
        public int Stock { get; set; }
        public float Price { get; set; }
    }
}
