using ETicaretAPI.Application.Abstractions.Hubs;
using ETicaretAPI.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ETicaretAPI.SignalR.HubServices;

public class ProductHubService : IProductHubService
{
    readonly IHubContext<ProductHub> _hubContext;

    public ProductHubService(IHubContext<ProductHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task ProductAddedMessageAsync(string message)
    {
        // Burdaki fonksiyonu tetiklediğimizde : Burdaki sokete , signalr yapılanmasına bağlı olan tüm clientlarda ilgili fonksiyon varsa tetiklenecektir.
        //SendAsync(hangi fonksiyona karşılık, mesaj ne) 
        await _hubContext.Clients.All.SendAsync(ReceiveFunctionNames.ProductAddedMessage, message);
    }
}
