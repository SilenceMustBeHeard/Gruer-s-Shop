using GruersShop.Data.Models.Messages;
using GruersShop.Data.Repositories.Interfaces.CRUD;

namespace GruersShop.Data.Repositories.Interfaces.Messages;

public interface IContactMessageRepository :
    IFullRepositoryAsync<ContactMessage, Guid>
{
    Task<List<ContactMessage>> GetAdminMessagesAsync(string adminId);
}