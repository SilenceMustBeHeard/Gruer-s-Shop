using GruersShop.Data.Models.Messages;
using GruersShop.Data.Repositories.Implementations.Base;
using GruersShop.Data.Repositories.Interfaces.Messages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Implementations.Messages;

public class SystemInboxMessageRepository : RepositoryAsync<SystemInboxMessage, Guid>, ISystemInboxMessageRepository
{
    public SystemInboxMessageRepository(AppDbContext context) : base(context)
    {
    }

    
   
}