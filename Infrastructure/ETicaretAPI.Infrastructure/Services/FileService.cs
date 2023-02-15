using ETicaretAPI.Application.Services;
using ETicaretAPI.Infrastructure.Operations;
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

                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                return true;
            }
            catch(Exception ex)
            {
                //todo log  = view - Task listten görebiliyoruz = loglama için
                throw ex;
            }
        }

        // dosya isimlendirmek için ayrı bir method oluşturuyorus solid prensiblerinde her iş için ayrı : methot - sınıfta yapma prensibine binaen
        //dışarıdan erişim istemiyoruz bu dosyaya ondan interface'den kaldırdık. == private yaptık sadece bu class'da kullnacaz.
        // fonksiyonda ilk girişti isimdeki boşluk gibi : hem seo çalışması hemde dosya kaydederken yol'da sorun çıkartacak karekterleri değiştiriyoruz gerekli yere yönlendirip : NameOperation.CharecterRegulatory , daha sonra ise : aynı dosya ismi kayıtlımı diye kontrol yapıyoruz : eğerki kayıtlı ise dosya ismi - 2 , -3 gibi taki benzer bir isim olmıyana kadar sonundaki ismi değiştireceğiz.
        private async Task<string> FileRenameAsync(string path,string fileName, bool first = true)
        {
            string newFileName = await Task.Run(async () =>
            {
                string extension = Path.GetExtension(fileName); // file namedeki extension'ı bana getir

                string newFileName = string.Empty;
                if (first) //ilk girişte buraya gir = sonraki seferlerde girme : aynı isimli dosya olma durumları.
                {
                    string oldName = Path.GetFileNameWithoutExtension(fileName); //extensiyon'suz ismini getir dosyanın
                    newFileName = $"{NameOperation.CharecterRegulatory(oldName)}{extension}";
                }
                else
                {
                    newFileName = fileName;
                    int indexNo1 = newFileName.IndexOf('-'); //eğerki newFileName string'inin içinde '-' yok ise -1 döndürür. var ise olduğu index'i döndürür = birden fazla var ise ilk gördüğü index'i döndürür
                    if (indexNo1 == -1)
                        newFileName = $"{Path.GetFileNameWithoutExtension(newFileName)}-2{extension}";
                    else
                    {
                        int lastIndex = 0;
                        while (true) // gelen dosya isminin içinde '-' karekterinin olması durumunda çıkan hatayı enggelemek için.
                        {
                            lastIndex = indexNo1;
                            indexNo1 = newFileName.IndexOf("-", indexNo1 + 1);
                            if(indexNo1 == -1)
                            {
                                indexNo1 = lastIndex;
                                break;
                            }
                        }

                        int indexNo2 = newFileName.IndexOf(".");
                        string fileNo = newFileName.Substring(indexNo1 + 1, indexNo2 - indexNo1 -1);

                        if(int.TryParse(fileNo, out int _fileNo)) //string'i sayısal değere dönüştürülebiliyor ise dönüştür.
                        {
                            _fileNo++;
                            //REMOVE : indexNo1 indexinden başla , (indexNo2 - indexNo1 - 1) bu index'e kadar sil
                            newFileName = newFileName.Remove(indexNo1 + 1 , indexNo2 - indexNo1 - 1)
                                                    .Insert(indexNo1 + 1 , _fileNo.ToString());
                            // Insert : indexNo1 = indexinden başla ; _fileNo.ToString() : bu veriyi ekle.
                        }
                        else
                        {
                            newFileName = $"{Path.GetFileNameWithoutExtension(newFileName)}-2{extension}";
                        }
                       
                    }
                }


                if (File.Exists($"{path}\\{newFileName}")) //ilk seferde manuel olarak 2. turdaki ismi ekliyorum.
                    return await FileRenameAsync(path, newFileName, false);
                else
                    return newFileName;
            });
            return newFileName; 
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
                string fileNewName = await FileRenameAsync(uploadPath, file.FileName);
                //wwwroot'a kaydetme işlemini gerçekleştirmek için methodumuz.
                bool result = await CopyFileAsync($"{uploadPath}\\{fileNewName}",file);
                datas.Add((fileNewName, $"{path}\\{fileNewName}"));
                results.Add(result);
            }

            if (results.TrueForAll(r => r.Equals(true))) //tüm results lar true ise
                return datas; // bütün dosyaların isimlerini ve yollarını döndür
            return null;
            
            //todo Eğer ki yukarıdaki if geçerli değilse burada dosyaların suncuda yüklenirken alındığına dair uyarıcı bir exception oluşturulup fırlatılması gerekiyor.
        }
    }
}
