using GruersShop.Data.Models.Messages;
using GruersShop.Data.Repositories.Implementations.Base;
using GruersShop.Data.Repositories.Interfaces.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Implementations.Messages;

public class ContactMessageRepository : RepositoryAsync<ContactMessage, Guid>, IContactMessageRepository
{
    public ContactMessageRepository(AppDbContext context) : base(context)
        { }
    
       public async Task<List<ContactMessage>> GetAdminMessagesAsync(string adminId)
    
        => await Query()
            .Include(m => m.Sender)
            .Include(m => m.RespondedBy)
            .Where(m => m.ReceiverId == adminId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    
}

  

