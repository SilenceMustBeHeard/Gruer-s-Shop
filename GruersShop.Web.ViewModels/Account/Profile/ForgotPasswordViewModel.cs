using System.ComponentModel.DataAnnotations;

namespace GruersShop.Web.ViewModels.Account.Profile;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;
}