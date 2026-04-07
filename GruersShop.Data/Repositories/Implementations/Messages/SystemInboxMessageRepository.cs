using GruersShop.Data.Models.Messages;
using GruersShop.Data.Repositories.Implementations.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Implementations.Messages;

public class SystemInboxMessageRepository : RepositoryAsync<SystemInboxMessage, int>
{
    public SystemInboxMessageRepository(DbContext context) : base(context)
    {
    }

    
   
}