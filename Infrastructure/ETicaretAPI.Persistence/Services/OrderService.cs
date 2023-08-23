using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Persistence.Services;

public class OrderService : IOrderService
{
    readonly IOrderWriteRepository _orderWriteRepository;
    readonly IOrderReadRepository _orderReadRepository;

    public OrderService(IOrderWriteRepository orderWriteRepository, IOrderReadRepository orderReadRepository)
    {
        _orderWriteRepository = orderWriteRepository;
        _orderReadRepository = orderReadRepository;
    }

    public async Task CreateOrderAsync(CreateOrder createOrder)
    {

        await _orderWriteRepository.AddAsync(new()
        {
            Address= createOrder.Address,
            Id = Guid.Parse(createOrder.BasketId),
            Description= createOrder.Description,
            OrderCode = (new Random().NextDouble() * 10000).ToString().Split('.')[1]//NextDouble => 0.1122125 => 0 - 1 arası bir değer alır.
        });
        await _orderWriteRepository.SaveAsync();
    }

    public async Task<ListOrder> GetAllOrderAsync(int page, int size)
    {
        var query = _orderReadRepository.Table
            .Include(o => o.Basket)
                .ThenInclude(b => b.User)
            .Include(o => o.Basket)
                .ThenInclude(b => b.BasketItems)
                .ThenInclude(bi => bi.Product);


        var data = query.Skip(page * size).Take(size);
        //.Take((page * size)..size); // (2*10)..(10) => 20. veriden itibaren 10 veri al => 20 - 30 

        return new()
        {
            TotalOrderCount = await query.CountAsync(),
            Orders = await data.Select(o => new
            {
                CreatedDate = o.CreatedDate,
                OrderCode = o.OrderCode,
                TotalPrice = o.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity),
                UserName = o.Basket.User.UserName
            }).ToListAsync()
        };
    }
}
