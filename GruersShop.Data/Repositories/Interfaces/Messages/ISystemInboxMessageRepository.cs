using GruersShop.Data.Models.Messages;
using GruersShop.Data.Repositories.Interfaces.CRUD;

namespace GruersShop.Data.Repositories.Interfaces.Messages;

public interface ISystemInboxMessageRepository
    : IFullRepositoryAsync<SystemInboxMessage, Guid>
{
}