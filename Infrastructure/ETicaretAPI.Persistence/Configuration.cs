using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence
{
    public class Configuration // bu sayfa jsondan verileri çekip statick değerleree koyma ve gerekli sayfalardan bu static değerleri çekmek için tasarlanmıştır.
    {
        // PostgreSQL bağlantı bilgisini api katmanındaki appsetting.json'dan çekmek için static değişken. == ilgili sayfalardan çağrılıyor bu değişken.
        static public string ConnectionString
        {
            get
            {
                ConfigurationManager configurationManager = new(); //.net 6 ile json dosyalarına erişmek için geldi sayfaya ekleyebilmek için bulunduğu katmana : nugetten Microsoft.Extensions.Configuration 'ı ekliyoruz 
                //eğerki farklı bir katmandaki jsona erişeceksek de(API katmanındaki json'a erişmek istiyorum ben.) : nugetten ilgii katmana Microsoft.Extensions.Configuration.Json'ı ekliyoruz yine. bu sayede alttaki 2 komutu girebiliyorum.
                configurationManager.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../Presentation/ETicaretAPI.API")); //json'a erişmek için yolu veriyorum
                configurationManager.AddJsonFile("appsettings.json"); // erişmek istediğim json ismini veiryorum
                return configurationManager.GetConnectionString("PostgreSQL");//json içindeki çekmek istediğim objenin key'ini yazıyorum
            }
        }
    }
}
