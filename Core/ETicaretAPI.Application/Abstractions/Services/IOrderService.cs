﻿using ETicaretAPI.Application.DTOs.Order;

namespace ETicaretAPI.Application.Abstractions.Services;

public interface IOrderService
{
    Task CreateOrderAsync(CreateOrder createOrder);
    Task<ListOrder> GetAllOrderAsync(int page, int size);
    Task<SingleOrder> GetOrderByIdAsync(string id);
    Task CompleteOrderAsync(string id);
}
