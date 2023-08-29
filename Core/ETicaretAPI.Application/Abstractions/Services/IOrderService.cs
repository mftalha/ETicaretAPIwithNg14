﻿using ETicaretAPI.Application.DTOs.Order;

namespace ETicaretAPI.Application.Abstractions.Services;

public interface IOrderService
{
    Task CreateOrderAsync(CreateOrder createOrder);
    Task<ListOrder> GetAllOrderAsync(int page, int size);
    Task<SingleOrder> GetOrderByIdAsync(string id);
    //tapple nesne deniyor yazılışı farklı olabilir
    Task<(bool, CompletedOrderDTO)> CompleteOrderAsync(string id);
}
