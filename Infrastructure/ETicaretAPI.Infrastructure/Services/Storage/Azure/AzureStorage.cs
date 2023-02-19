using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ETicaretAPI.Application.Abstractions.Stroge.Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Storage.Azure
{
    public class AzureStorage : IAzureStorage
    {
        readonly BlobServiceClient _blobServiceClient; //ilgili azure acocountuna bağlanmamıza sağlarken
        BlobContainerClient _blobContainerClient; //hedef account üzerinde ilgili dosya işlemlerini yapmamızı sağlıyor.
        public AzureStorage(IConfiguration configuration)
        {
            _blobServiceClient = new(configuration["Storage:Azure"]);
        }
        public Task DeleteAsync(string ContainerName, string fileName)
        {
            throw new NotImplementedException();
        }

        public List<string> GetFiles(string ContainerName)
        {
            throw new NotImplementedException();
        }

        public bool HasFile(string ContainerName, string fileName)
        {
            throw new NotImplementedException();
        }

        //parametre isimleri değişebilir ama tappıl içindeki değişken isimleri de interfaceden gelen metotlarda.
        public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string ContainerName, IFormFileCollection files)
        {
            //hangi container'da çalışacağımı belirtiypruz.
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
            await _blobContainerClient.CreateIfNotExistsAsync(); // Container yok sa : ilgili container'ı oluştur.
            // Blob Container'ına erişim izni veriyoruz.
            await _blobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

            List<(string fileName, string pathOrContainerName)> datas = new();
            foreach (IFormFile file in files) //gelen files'lar arasında dolaş.
            {
                BlobClient blobClient = _blobContainerClient.GetBlobClient(file.Name);//Göndereceğim dosyanın adı bu.
                // dosyayı akışa çevir göder azure'ye
                await blobClient.UploadAsync(file.OpenReadStream());
            }
        }
    }
}
