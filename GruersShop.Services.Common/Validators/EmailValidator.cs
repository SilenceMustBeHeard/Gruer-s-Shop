using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace GruersShop.Services.Common.Validators;

public class EmailValidator
{
    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {

            var mailAddress = new MailAddress(email);
            return mailAddress.Address == email.Trim();
        }
        catch
        {
            return false;
        }
    }

    public string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return string.Empty;


        return email.Trim();
    }
}