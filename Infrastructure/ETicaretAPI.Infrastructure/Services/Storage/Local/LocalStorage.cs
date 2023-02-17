using ETicaretAPI.Application.Abstractions.Stroge.Local;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Storage.Local
{
    public class LocalStorage : ILocalStorage
    {

        readonly IWebHostEnvironment _webHostEnviroment; // wwwroot path'ine erişim için kullanıyoruz.
        public LocalStorage(IWebHostEnvironment webHostEnviroment)
        {
            _webHostEnviroment = webHostEnviroment;
        }

        public async Task DeleteAsync(string path, string fileName)
            => File.Delete($"{path}\\{fileName}");
        

        public List<string> GetFiles(string path)
        {
            DirectoryInfo directory = new(path); //ilgili dizine gidiyorum
            return directory.GetFiles().Select(f => f.Name).ToList(); // sadece dosya isimlerini geri döndürüyorum.
        }

        public bool HasFile(string path, string fileName)
            => File.Exists($"{path}\\{fileName}");

        async Task<bool> CopyFileAsync(string path, IFormFile file)
        {
            try
            {
                //using kullandığımız için methotla işimiz bittiğinde methodda kullanılan nesneleri dispose(elden çıkarma) edecektir.
                await using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false);

                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                return true;
            }
            catch (Exception ex)
            {
                //todo log  = view - Task listten görebiliyoruz = loglama için
                throw ex;
            }
        }
        
        public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string path, IFormFileCollection files)
        {
            
            // Path.Combine(_webHostEnvironment.WebRootPath,"ahmet"); == wwwroot/ahmet
            string uploadPath = Path.Combine(_webHostEnviroment.WebRootPath, path);
            if (!Directory.Exists(uploadPath)) // ilgili path'de klasör mevcut değilse klasör'ü oluştur.
                Directory.CreateDirectory(uploadPath);

            List<(string fileName, string path)> datas = new();
            foreach (IFormFile file in files)
            {
                //wwwroot'a kaydetme işlemini gerçekleştirmek için methodumuz.
                 await CopyFileAsync($"{uploadPath}\\{file.Name}", file);
                datas.Add((file.Name, $"{path}\\{file.Name}"));
            }

            return datas;
            //todo Eğer ki yukarıdaki if geçerli değilse burada dosyaların suncuda yüklenirken alındığına dair uyarıcı bir exception oluşturulup fırlatılması gerekiyor.
            
        }
        

        /*
        public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string path, IFormFileCollection files)
        {
            string uploadPath = Path.Combine(_webHostEnviroment.WebRootPath, path);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            List<(string fileName, string path)> datas = new();
            foreach (IFormFile file in files)
            {
                string fileNewName = await FileRenameAsync(path, file.Name, HasFile);

                await CopyFileAsync($"{uploadPath}\\{fileNewName}", file);
                datas.Add((fileNewName, $"{path}\\{fileNewName}"));
            }

            return datas;
        }
        */
    }
}
