using ETicaretAPI.Domain.Entities.Common;

namespace ETicaretAPI.Domain.Entities;

// tamamlanmış siparişleri tutacağız
public class CompletedOrder : BaseEntity
{
    public Guid OrderID { get; set; }
    public Order Order { get; set; }

}
