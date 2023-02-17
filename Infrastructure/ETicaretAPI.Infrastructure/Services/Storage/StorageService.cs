using ETicaretAPI.Application.Abstractions.Stroge;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Storage
{
    // Bu sayfa storage deki yüzümüz olacak.
    public class StorageService : IStorageService
    {
        readonly IStorage _storage; //mimaride kullanılan storage yi çek : Depencedcy enjection vasıtası ile == hangi storage kullanılacağını : Presentation katmanındaki : Program.cs içinde seçecem : builder.Services.AddStorage(Storage.Azure); == gibi 
        public StorageService(IStorage storage)
        {
            _storage = storage;
        }

        public string StrogeName { get => _storage.GetType().Name; } //hangi storage kullanılıyor ise onun ismi dönecektir  : Local , azure

        public async Task DeleteAsync(string pathOrContainerName, string fileName)
            => _storage.DeleteAsync(pathOrContainerName, fileName); //hangi depolama kullanılıyor ise ondaki DeleteAsync fonksiyonunu kullan.

        public List<string> GetFiles(string pathOrContainerName)
            => _storage.GetFiles(pathOrContainerName);

        public bool HasFile(string pathOrContainerName, string fileName)
            => _storage.HasFile(pathOrContainerName, fileName);

        public  Task <List<(string fileName, string pathOrContainerName)>> UploadAsync(string pathOrContainerName, IFormFileCollection files)
            =>  _storage.UploadAsync(pathOrContainerName, files);
    }
}
