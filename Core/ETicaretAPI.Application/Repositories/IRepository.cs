using ETicaretAPI.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories
{
    // where T: class yerine BaseEntity'den türetiyoruz çünkü id ye göre okuma yaptığımız GetByIdAsync(string id) methodundaki id yi beliremiyoruz diğer türlü : bütün entityler BaseEntity den türediği için BaseEntity dediğimde = zaten entity bekliyecek bide generic type : çağrılan class , interface dan türeyen class , interface leride kabul ediyor t paremetresi olarak = bu yöntemin adıda = Marker(işaretleyici) olarak geçiyor : yani bir class yada interfaceden türet diğer entities leri böylece bir id veya başka bir değiştken üzerinden aratma yapmak gerektiğinde o objeyi kullanabil yoksa bütün T(entity) lerde ortak olarak arka planda id olduğunu sistem bilmez ve veritabanı katmanında id objesine erişemediğimde id ye göre aratma işlemini yapamam. = bi yöntem daha varmış bu işlemi yapmak için == reflection ama bu yöntemi kullanmak çok uzun zaman alıyormuş. o yüzden direk bu şekilde yapmak daha mantıklıymış.
    //<> : bu yapının ismi Generic diye geçiyor : burda farklı type da (enum , class , interface  ...) farklı isimlerde(a classı , b enumu) dosyaların gelebileceğini belirtmek için kullanıyruz. // T yerine ahmet de yazabilriz : T yazma sebebimiz Type nin T si belirtmek için sadece.
    //where T: class deme sebibimiz gelen T nin class olacağını belirtiyoruz yoksa : T : enum i struct ,  class ... olabilir ama bizim kullandığımız DbSet sadece class istiyor(entity) o yüzden class oalcak diye belirtiyoruz.
    //bütün veritabanları tarafından ortak kullanılacak şeyleri burada tanımlıyoruz. Burada <T> verme sebimiz : hangi entityde çalışılıyor ise o gelsin diye : order, customer .. olabilir.: birlirli bi entity vermiyoruzda gelen entity'e göre işlem yap diye belitiyoruz. :Gener,c Repository dfeki amaçda bu.
    public interface IRepository<T> where T : BaseEntity
    {
        DbSet<T> Table { get; } //DbSet'de biz Tablemizi alırız. ama herhangi bir set işlemi yapmayız
    }
}
