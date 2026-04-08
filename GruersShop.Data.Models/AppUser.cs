
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GruersShop.Data.Models;

public class AppUser : IdentityUser
{
    public string FullName => $"{FirstName?.Trim()} {LastName?.Trim()}".Trim();

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string AlternateEmail { get; set; } = null!;
}
