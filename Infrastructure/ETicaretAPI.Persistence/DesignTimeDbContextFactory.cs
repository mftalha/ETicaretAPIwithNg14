using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence
{ //bu sayfayı oluşturma sebebimiz cmd veya shell üzerinden migration oluşturmak için : bazen duruma göre gerekebiliyormuş mesela visual studio kullanılamıyacak durumda.
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ETicaretAPIDbContext>
    {
        public ETicaretAPIDbContext CreateDbContext(string[] args) 
        {
            DbContextOptionsBuilder<ETicaretAPIDbContext> dbContextOptionsBuilder = new();
            //dbContextOptionsBuilder.UseNpgsql("User ID=postgres;Password=123456;Host=localhost;Port=5432;Database=ETicaretAPIDb;"); // connection stringi başka sayfalardada giriyorum : aynı komutu tekrarlamamak için appsetting.json 'a koydum connection stringi ve ordan çektim. o yüzden bu kullanıma gerek kalmadıki . doğru bi kullanımda değil bu kullanum.
            //UseNpgsql kullanabilmek için nugetten Npgsql.EntityFrameworkCore.PostgreSQL paketi indirilmelidir. 
            dbContextOptionsBuilder.UseNpgsql(Configuration.ConnectionString);
            return new(dbContextOptionsBuilder.Options);
        }
    }
}
