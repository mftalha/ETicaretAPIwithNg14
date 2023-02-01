using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Filters
{
    public class ValidationFilter : IAsyncActionFilter // action'a gelen isteklerde devreye giren bir filtre
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(!context.ModelState.IsValid) //eğerki gelen verilere göre validation true değil ise.
            {
                var errors = context.ModelState
                    .Where(x => x.Value.Errors.Any()) //eğerki veri var ise
                    .ToDictionary(e => e.Key, e => e.Value.Errors.Select(e => e.ErrorMessage))
                    .ToArray();
                /*
                 * e.Key bize ilgili property'i getirecek ; e.Value.. = o propert'e ayit tüm hataları getirecek.
                 */
                context.Result = new BadRequestObjectResult(errors);
                return; // hatalı bir validation durumu varsa bir sonraki validation kontrolüne geçme. : bitir.
            }

            await next(); // filter yapıları : sıralı bir şekilde çalışır :: next fonksiyonu = bir sonraki filteri temsil ediyor o yüzden işlem bittikten sonra next demem gerekiyor.

            //mantık : biz burda gelen verilerde validationları tek tek konrol edecez : kontrolü if ile bakıyoruz eğerki sorunlu bi durum varsa if'e giirip kullanıcıya gerekli dönüşü sağlıyacaz. : sorun yoksa da : next ile bir sonraki validationa geçecez.
        }
    }
}
