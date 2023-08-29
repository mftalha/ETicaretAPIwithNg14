using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Persistence.Contexts;

namespace ETicaretAPI.Persistence.Repositories;

public class ComplatedOrderReadRepository : ReadRepository<CompletedOrder>, ICompletedOrderReadRepository
{
    public ComplatedOrderReadRepository(ETicaretAPIDbContext context) : base(context)
    {
    }
}
