using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Persistence.Contexts;

namespace ETicaretAPI.Persistence.Repositories;

public class ComplatedOrderWriteRepository : WriteRepository<CompletedOrder>, ICompletedOrderWriteRepository
{
    public ComplatedOrderWriteRepository(ETicaretAPIDbContext context) : base(context)
    {
    }
}
