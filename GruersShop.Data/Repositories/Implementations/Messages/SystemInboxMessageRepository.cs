using GruersShop.Data.Models.Messages;
using GruersShop.Data.Repositories.Implementations.Base;
using GruersShop.Data.Repositories.Interfaces.Messages;

namespace GruersShop.Data.Repositories.Implementations.Messages;

public class SystemInboxMessageRepository
    : RepositoryAsync<SystemInboxMessage, Guid>, ISystemInboxMessageRepository
{
    public SystemInboxMessageRepository(AppDbContext context) : base(context)
    {
    }
}