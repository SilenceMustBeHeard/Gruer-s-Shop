using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Interactions;

public class ReviewViewModel
{

    public Guid Id { get; set; }
    public string UserName { get; set; } = "Anonymous";
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string FormattedDate => CreatedAt.ToLocalTime().ToString("dd MMM yyyy");
    public string StarRating => new string('★', Rating) + new string('☆', 5 - Rating);
}
