using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Application.ViewModel.Product;
using ETicaretAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly private IProductWriteRepository _productWriteRepository;
        readonly private IProductReadRepository _productReadRepository;
        private readonly IWebHostEnvironment _webHostEnvironment; // wwwroot path'ine erişim için kullanıyoruz.

        public ProductsController(
            IProductWriteRepository productWriteRepository,
            IProductReadRepository productReadRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            this._webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Pagination pagination)
        {
            
            var totalCount = _productReadRepository.GetAll(false).Count();
            //skip ile kaçıncı veriye kadar alınacağını söylüyorum mesela 15 ; take ile de 5 tane alınacaksa = 10 - 15 arası veriler getiriliyor gibi bi mantık anlıyorum.
            var products = _productReadRepository.GetAll(false).Skip(pagination.Page * pagination.Size).Take(pagination.Size).Select(p => new
            { // bu method çağrıldığında bütün tablo verilerini değilde sadece bu verileri döndür diyoruz.
                p.Id,
                p.Name,
                p.Stock,
                p.Price,
                p.CreatedDate,
                p.UpdatedDate
            }).ToList(); // 5(sayfada gösterim adedi) *3(kaçıncı sayfa) = 15 veriyi getir ;; Skip(pagination.Size) == bu kadar veriyi getir gibi mantık galiba.

            return Ok(new
            {
                totalCount,
                products
            }); //tracking i falseye çekiyorum. defaultu true idi.
            /* tüm verileri döndürmek için.
            var result = _productReadRepository.GetAll(false);
            return Ok(result);
            */
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _productReadRepository.GetByIdAsync(id, false));
        }


        //dış dünyadan gelecek prodoct işlemlerini entity ile karşılamıyacam view model ile karşılayıp ona göre işleme devam edecem.
        [HttpPost]
        public async Task<IActionResult> Post(VM_Create_Product model)
        {
            await _productWriteRepository.AddAsync(new()
            {
                Name = model.Name,
                Price = model.Price,
                Stock = model.Stock
            });
            await _productWriteRepository.SaveAsync();
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPut]
        public async Task<IActionResult> Put(VM_Update_Product model)
        {
            Product product = await _productReadRepository.GetByIdAsync(model.Id);
            product.Stock = model.Stock;
            product.Name = model.Name;
            product.Price = model.Price;
            await _productWriteRepository.SaveAsync();
            return Ok();
        }

        [HttpDelete("{id}")] //id diye bir paremetre gelecek diye belirtiyoruz.
        public async Task<IActionResult> Delete(string id)
        {
            await _productWriteRepository.RemoveAsync(id);
            await _productWriteRepository.SaveAsync();
            return Ok();
        }

        [HttpPost("[action]")] // birden fazla post methodu olduğu için action ismi ile ayırmamız gerekiyor : şart = bizde kendi adını veriyoruz action kısmına.
        public async Task<IActionResult> Upload()
        {
             
            // Path.Combine(_webHostEnvironment.WebRootPath,"ahmet"); == wwwroot/ahmet
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath,"resource/product-images");

            if (!Directory.Exists(uploadPath)) // ilgili path'de klasör mevcut değilse klasör'ü oluştur.
                Directory.CreateDirectory(uploadPath);

            Random r = new(); //random isim alabilmek için
            //Request.Form.Files  // clientten gelen FormData ları burda yakalıyacaz : method paremetresi olarak yakalamıyoruz burda yakalıyoruz dikkkat! : collection olarak geldiği için foreach ile kullanabileceğim.
            foreach(IFormFile file in Request.Form.Files)
            {
                //file.Name = a.png deki : a yı alır sadece uzantısını almaz. ,,, file.FileName ilede = a.png : deki png yi alır : yani type'ı alır.
                string fullPath = Path.Combine(uploadPath, $"{r.Next()}{Path.GetExtension(file.FileName)}"); // dosyaların isimlerinin benzersiz olmasını sağlamak için nereye kaydedileceği + random isimler + type larını verdim : geçiçi çözüm burası. == full path

                using FileStream fileStream = new(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false); //gerekli stream işlemleri(propert'yler)
                await file.CopyToAsync(fileStream); // hedef stram'a basacam
                await fileStream.FlushAsync(); //stream'ı temizliyorum.
            }
            return Ok();
        }
    }
}
