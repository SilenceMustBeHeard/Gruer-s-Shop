using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GruersShop.Web.ViewModels.Account.Profile;

public class RegisterViewModel
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = null!;


    [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "First name can only contain letters, numbers, spaces, and hyphens.")]
    public string? FirstName { get; set; }    // Personalization Purposes
    [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Last name can only contain letters, numbers, spaces, and hyphens.")]
    public string? LastName { get; set; }     // by choice
    [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Address can only contain letters, numbers, spaces, and hyphens.")]
    public string? Address { get; set; }     // by choice

    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string AlternateEmail { get; set; } = null!;    

    [Required]
    [DataType(DataType.Password)]
    [Compare("ConfirmPassword", ErrorMessage = "Passwords do not match.")]
    public string Password { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = null!;
}
