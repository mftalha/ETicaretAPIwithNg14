using ETicaretAPI.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace ETicaretAPI.Infrastructure.Services
{
    public class FileService : IFileService
    {
        readonly IWebHostEnvironment _webHostEnviroment; // wwwroot path'ine erişim için kullanıyoruz.
        public FileService(IWebHostEnvironment webHostEnviroment)
        {
            _webHostEnviroment = webHostEnviroment;
        }

        public async Task<bool> CopyFileAsync(string path, IFormFile file)
        {
            try
            {
                //using kullandığımız için methotla işimiz bittiğinde methodda kullanılan nesneleri dispose(elden çıkarma) edecektir.
                await using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false);

                await fileStream.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                return true;
            }
            catch(Exception ex)
            {
                //todo log  = view - Task listten görebiliyoruz = loglama için
                throw ex;
            }
        }

        public Task<string> FileRenameAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        public async Task<List<(string fileName, string path)>> UploadAsync(string path, IFormFileCollection files)
        {
            // Path.Combine(_webHostEnvironment.WebRootPath,"ahmet"); == wwwroot/ahmet
            string uploadPath = Path.Combine(_webHostEnviroment.WebRootPath, path);
            if(!Directory.Exists(uploadPath)) // ilgili path'de klasör mevcut değilse klasör'ü oluştur.
                Directory.CreateDirectory(uploadPath);

            List<(string fileName, string path)> datas = new();
            List<bool> results = new();
            foreach(IFormFile file in files)
            {
                //isimlendirme methoduna gönderiyourz ve kaydetme ismini alıyorum.
                string fileNewName = await FileRenameAsync(file.FileName);
                //wwwroot'a kaydetme işlemini gerçekleştirmek için methodumuz.
                bool result = await CopyFileAsync($"{uploadPath}\\{fileNewName}",file);
                datas.Add((fileNewName, $"{uploadPath}\\{fileNewName}"));
                results.Add(result);
            }

            if (results.TrueForAll(r => r.Equals(true))) //tüm results lar true ise
                return datas; // bütün dosyaların isimlerini ve yollarını döndür
            return null;
            
            //todo Eğer ki yukarıdaki if geçerli değilse burada dosyaların suncuda yüklenirken alındığına dair uyarıcı bir exception oluşturulup fırlatılması gerekiyor.
        }
    }
}
