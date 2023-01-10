using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories
{
    // ReadRepository<Customer> , == şeklinde virgülden önce boşluk verdiğimde hata veriyor. 
    // , ICustomerReadRepository kullanma sebebim  = CustomerReadRepository clasının doğrulayıcı , imzalıyıcısıdır ve depending enjection'dan  CustomerReadRepository'i çağırırken  ICustomerReadRepository kullanacaz.
    // : ReadRepository<Customer> kullanma sebebimiz ise =  CustomerReadRepository'ın içini doldurmak içindir.
    // ICustomerReadRepository = imza ; ReadRepository<Customer> = doldurma , uygulama
    // ReadRepository<Customer> kaldırsam = ICustomerReadRepository 'ı uygulamamış olurum.
    // ReadRepository<Customer> 'ı koymamı sağlayanda  = ICustomerReadRepository'dir. == doğrulamasıdır.
    // Son olarak CustomerReadRepository'i ICustomerReadRepository ile depending enjection'a atacam ve ICustomerReadRepository anahtarı ile artık kullanabilecem.
    public class CustomerReadRepository : ReadRepository<Customer>, ICustomerReadRepository
    {

        // ReadRepository constracterında bizden  tane paremetre istiyordu = context == bu yüzden CustomerReadRepository'ın constracterından talep edip vermem gerekiyor bunu = yoksa hata verir zaten.
        // (ETicaretAPIDbContext context) bu nesne IOS'İ Containerdan gelirken ; base(ReadRepository) burdaki contex'i göndermek zorundayız. onuda buradan gönderiyoruz.
        public CustomerReadRepository(ETicaretAPIDbContext context) : base(context) { }
    }
}
