using ETicaretAPI.Application.Abstractions.Stroge;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.Repositories.File;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Application.ViewModels.Product;
using ETicaretAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
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
        readonly IFileReadRepository _fileReadRepository;
        readonly IFileWriteRepository _fileWriteRepository;
        readonly IProductImageFileReadRepository _productImageFileReadRepository;
        readonly IProductImageFileWriteRepository _productImageFileWriteRepository;
        readonly IInvoiceFileReadRepository _invoiceFileReadRepository;
        readonly IInvoiceFileWriteRepository _invoiceFileWriteRepository;
        readonly IStorageService _storageService;
        readonly IConfiguration configuration;
        public ProductsController(
            IProductWriteRepository productWriteRepository,
            IProductReadRepository productReadRepository,
            IWebHostEnvironment webHostEnvironment,
            IFileReadRepository fileReadRepository,
            IFileWriteRepository fileWriteRepository,
            IProductImageFileReadRepository productImageFileReadRepository, IProductImageFileWriteRepository productImageFileWriteRepository, IInvoiceFileReadRepository invoiceFileReadRepository,
            IInvoiceFileWriteRepository invoiceFileWriteRepository, IStorageService storageService)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            this._webHostEnvironment = webHostEnvironment;
            _fileReadRepository = fileReadRepository;
            _fileWriteRepository = fileWriteRepository;
            _productImageFileReadRepository = productImageFileReadRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
            _invoiceFileReadRepository = invoiceFileReadRepository;
            _invoiceFileWriteRepository = invoiceFileWriteRepository;
            _storageService = storageService;
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
        // ...com/api/products?id=123  => querystring // birden fazla da olabilir deger alma belirsiz
        public async Task<IActionResult> Upload(string id)
        {
            List<(string fileName, string pathOrContainerName)> result = await _storageService.UploadAsync("photo-images", Request.Form.Files);

            Product product = await _productReadRepository.GetByIdAsync(id);

            /* : resim eklemede bunu da kullanabiliyoruz aşşağıdaki yapıya alternatif
            foreach(var r in result)
            {
                product.ProductImageFiles.Add(new()
                {
                    FileName = r.fileName,
                    Path = r.pathOrContainerName,
                    Storage = _storageService.StrogeName,
                    Products = new List<Product>() { product}
                });
            }
            */
            
            await _productImageFileWriteRepository.AddRangeAsync(result.Select(r => new ProductImageFile
            {
                FileName = r.fileName,
                Path = r.pathOrContainerName,
                Storage = _storageService.StrogeName,
                Products = new List<Product>() { product }
            }).ToList());
            
            await _productImageFileWriteRepository.SaveAsync();
            return Ok();
        }

        [HttpGet("[action]/{id}")] // ..com/api/products/123  : root data => ne geleceği belli ise
        public async Task<IActionResult> GetProductImages(string id)
        {
            Product? product = await _productReadRepository.Table.Include(p => p.ProductImageFiles)
                .FirstOrDefaultAsync(p => p.Id == Guid.Parse(id));
            
            return Ok(product!.ProductImageFiles.Select(p => new
            {
                Path = $"{configuration["BaseStorageUrl"]}/{p.Path}",
                p.FileName,
                p.Id
            }));
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteProductImage(string id, string imageId)
        {
            Product? product = await _productReadRepository.Table.Include(p => p.ProductImageFiles)
                .FirstOrDefaultAsync(p => p.Id == Guid.Parse(id));

            ProductImageFile productImageFile = product.ProductImageFiles.FirstOrDefault(p => p.Id == Guid.Parse(imageId));
            product.ProductImageFiles.Remove(productImageFile);
            await _productImageFileWriteRepository.SaveAsync();
            return Ok();
        }
    }
}
