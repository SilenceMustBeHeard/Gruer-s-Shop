using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Interfaces.Account;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body);
}