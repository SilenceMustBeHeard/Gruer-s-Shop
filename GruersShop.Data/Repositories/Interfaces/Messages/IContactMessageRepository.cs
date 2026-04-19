using GruersShop.Data.Models.Messages;
using GruersShop.Data.Repositories.Interfaces.CRUD;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Interfaces.Messages
{
    public interface IContactMessageRepository:
        IFullRepositoryAsync<ContactMessage, Guid>
    {
        
    }
}
