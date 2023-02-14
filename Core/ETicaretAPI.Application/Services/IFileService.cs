using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Services
{
    public interface IFileService //File servis için onion mimarisine binaen iskelet oluşturuyoruz.
    {
        // dosya ismi ve path bilgisi döndürsün : nereye kaydettiğimize dair = veritabanına koyarken lazım olabilir.
        Task<List<(string fileName, string path)>> UploadAsync(string path, IFormFileCollection files); // wwwroot'tan sonraki yol : path'de olsun , files = clientten gelen dosya türü galiba.
        Task<bool> CopyFileAsync(string path, IFormFile file); // dizine kaydetme işlemi için. yine aynı mantık solid prensibine binaen ayrı bir methotta.
    }
}
