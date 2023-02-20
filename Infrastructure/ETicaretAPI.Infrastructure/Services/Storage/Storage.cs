using ETicaretAPI.Infrastructure.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Storage
{
    //bütün depolama yerleri için ortak olup değişmeyecek methotlar buradan gelecek.
    // bütün depolama yerleri için farklı olup değeşecek methotlar interfaceye konup tüm depolama class'ları : miras alacak.
    public class Storage 
    {
        // hangi concrate üzerininde çalışıyor isem onu temsil edip : FileRenameAsync methodu içinde ona göre File.Exists kısmını düzenleyeceğim.
        protected delegate bool HasFile(string pathOrContainerName, string fileName);

        // dosya isimlendirmek için ayrı bir method oluşturuyorus solid prensiblerinde her iş için ayrı : methot - sınıfta yapma prensibine binaen
        //dışarıdan erişim istemiyoruz bu dosyaya ondan interface'den kaldırdık. == private yaptık sadece bu class'da kullnacaz.
        // fonksiyonda ilk girişti isimdeki boşluk gibi : hem seo çalışması hemde dosya kaydederken yol'da sorun çıkartacak karekterleri değiştiriyoruz gerekli yere yönlendirip : NameOperation.CharecterRegulatory , daha sonra ise : aynı dosya ismi kayıtlımı diye kontrol yapıyoruz : eğerki kayıtlı ise dosya ismi - 2 , -3 gibi taki benzer bir isim olmıyana kadar sonundaki ismi değiştireceğiz.
        // sadece kalıtımsal olarak erişilebilirsin diye protected olarak işaretliyoruz.
        protected async Task<string> FileRenameAsync(string pathOrContainerName, string fileName, HasFile hasFileMethod, bool first = true)
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
                            if (indexNo1 == -1)
                            {
                                indexNo1 = lastIndex;
                                break;
                            }
                        }

                        int indexNo2 = newFileName.IndexOf(".");
                        string fileNo = newFileName.Substring(indexNo1 + 1, indexNo2 - indexNo1 - 1);

                        if (int.TryParse(fileNo, out int _fileNo)) //string'i sayısal değere dönüştürülebiliyor ise dönüştür.
                        {
                            _fileNo++;
                            //REMOVE : indexNo1 indexinden başla , (indexNo2 - indexNo1 - 1) bu index'e kadar sil
                            newFileName = newFileName.Remove(indexNo1 + 1, indexNo2 - indexNo1 - 1)
                                                    .Insert(indexNo1 + 1, _fileNo.ToString());
                            // Insert : indexNo1 = indexinden başla ; _fileNo.ToString() : bu veriyi ekle.
                        }
                        else
                        {
                            newFileName = $"{Path.GetFileNameWithoutExtension(newFileName)}-2{extension}";
                        }

                    }
                }


                //if (File.Exists($"{path}\\{newFileName}")) //ilk seferde manuel olarak 2. turdaki ismi ekliyorum.
                if (hasFileMethod(pathOrContainerName, newFileName)) 
                    return await FileRenameAsync(pathOrContainerName, newFileName, hasFileMethod, false);
                else
                    return newFileName;
            });
            return newFileName;
        }
    }
}
