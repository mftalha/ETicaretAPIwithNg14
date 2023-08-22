using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.ViewModels.Basket;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Persistence.Services;

// bizim mantığımız her kullanıcının birden fazla basketi olabilir ama 1 tanesi aktif olur : tamamlananlar => orders tablosunda belirtilir => böylece 1 kullanıcı sipariş vereceği zaman kontrol yapacaz basket tablosunda o kullanıcının boş bir basketi varmı(böylece daha önce girip tamamlamadığı siparişleride göstermiş olacağız böylece)(verilmemiş basketinini olup olmadığınıda order'ı boşmu kontrolü ile anlıyoruz.). eğerki bir basket'i yoksada oluşturacağız. gibi bir basket mantığımız var. basket tablosundaki basketlerinde => basketItem tablosunda ürünlerini tutacağız 1 n ile(order - basket de 1-n)
public class BasketService : IBasketService
{
    // bu servisin kullanılabilmesi için program.cs ye ilgili inject yapılmalı = builder.Services.AddHttpContextAccessor();
    readonly IHttpContextAccessor _httpContextAccessor;
    readonly UserManager<AppUser> _userManager;
    readonly IOrderReadRepository _orderReadRepository;
    readonly IBasketWriteRepository _basketWriteRepository;
    readonly IBasketReadRepository _basketReadRepository;
    readonly IBasketItemWriteRepository _basketItemWriteRepository;
    readonly IBasketItemReadRepository _basketItemReadRepository;

    public BasketService(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IBasketItemWriteRepository basketItemWriteRepository, IOrderReadRepository orderReadRepository, IBasketWriteRepository basketWriteRepository, IBasketItemReadRepository basketItemReadRepository, IBasketReadRepository basketReadRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _orderReadRepository = orderReadRepository;
        _basketWriteRepository = basketWriteRepository;
        _basketItemWriteRepository = basketItemWriteRepository;
        _basketItemReadRepository = basketItemReadRepository;
        _basketReadRepository = basketReadRepository;
    }

    private async Task<Basket?> ContextUser()
    {
        // Controllarda Authorize verilmez ise patlar çünkü jwt içinde dolduruyoruz bu veriyi.
        var usurnama = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(usurnama))
        {
            AppUser? user = await _userManager.Users
                .Include(u => u.Baskets)
                .FirstOrDefaultAsync(u => u.UserName== usurnama);

            var _basket = from basket in user.Baskets
                          join order in _orderReadRepository.Table
                          on basket.Id equals order.Id into BasketOrders
                          from order in BasketOrders.DefaultIfEmpty()
                          select new
                          {
                              Basket =basket,
                              Order = order
                          };

            Basket? targetBasket = null;
            if (_basket.Any(b => b.Order is null))
                // b => b.orders is null => order'ı null olan basketi veriyoruz.
                targetBasket = _basket.FirstOrDefault(b => b.Order is null)?.Basket;
            else
            {
                targetBasket = new();
                user.Baskets.Add(targetBasket);
            }
            await _basketWriteRepository.SaveAsync(); // bu elsenin içinde olması gerekebilir.
            return targetBasket;
        }
        throw new Exception("Beklenmedik bir hatayla karşılaşıldı...");
    }

    public async Task AddItemToBasketAsync(VM_Create_BasketItem basketItem)
    {
        Basket? basket = await ContextUser();
        if(basket != null)
        {
            BasketItem _basketItem = await _basketItemReadRepository.GetSingleAsync(bi => bi.BasketId == basket.Id && bi.ProductId == Guid.Parse(basketItem.ProductId));
            // eğerki ilgili basket item daha önce eklenmiş ise sadece quantity'i 1 arttırıyoruz
            if (_basketItem != null)
                _basketItem.Quantity++;
            else
                await _basketItemWriteRepository.AddAsync(new()
                {
                    BasketId = basket.Id,
                    ProductId = Guid.Parse(basketItem.ProductId),
                    Quantity = basketItem.Quentity
                });

            await _basketItemWriteRepository.SaveAsync();
        }
    }

    public async Task<List<BasketItem>> GetBasketItemsAsync()
    {
        Basket? basket = await ContextUser();
        Basket? result = await _basketReadRepository.Table
            .Include(b => b.BasketItems)
            .ThenInclude(bi => bi.Product)
            .FirstOrDefaultAsync(b => b.Id == basket.Id);

        return result.BasketItems
            .ToList();
    }

    public async Task RemoveBasketItemAsync(string basketItemId)
    {
        BasketItem? basketItem = await _basketItemReadRepository.GetByIdAsync(basketItemId);
        if (basketItem != null)
        {
            await _basketItemWriteRepository.RemoveAsync(basketItem.Id.ToString());
            // await _basketItemWriteRepository.Remove(basketItem); //gencay yıldızın yaptıgı
            await _basketItemWriteRepository.SaveAsync();
        }
    }

    public async Task UpdateQuantityAsync(VM_Update_BasketItem basketItem)
    {
        BasketItem? _basketItem = await _basketItemReadRepository.GetByIdAsync(basketItem.BasketItemId);
        if(_basketItem != null)
        {
            _basketItem.Quantity = basketItem.Quantity;
            await _basketItemWriteRepository.SaveAsync();
        }
    }

    public Basket? GetUserActiveBasket
    {
        get
        {
            Basket? basket =  ContextUser().Result;
            return basket;
        }
    }
}
