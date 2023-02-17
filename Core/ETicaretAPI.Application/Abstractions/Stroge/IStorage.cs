using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Stroge
{
    //tüm kaydetme sunucuları için ortak methotlar : awm , azure storage , local storage ...
    public interface IStorage
    {
        // dosya ismi ve path bilgisi döndürsün : nereye kaydettiğimize dair = veritabanına koyarken lazım olabilir.
        //azure da Blobstorage(depolama için) kullnacaz : ve azurdaki dosyaların karşılığı Container deniyor.
        Task <List<(string fileName, string pathOrContainerName)>> UploadAsync(string pathOrContainerName, IFormFileCollection files); // wwwroot'tan sonraki yol : path'de olsun , files = clientten gelen dosya türü galiba.
        Task DeleteAsync(string pathOrContainerName, string fileName);
        List<string> GetFiles(string pathOrContainerName);
        bool HasFile(string pathOrContainerName, string fileName);
    }
}
