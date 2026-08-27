using System.ComponentModel.DataAnnotations;

namespace AIPlacement.MVC.Models.CompanyAndJob;

public class CompanyProfileFormViewModel
{
    public int CompanyId { get; set; }
    public int UserId { get; set; }

    [Required, StringLength(150)]
    [Display(Name = "Company Name")]
    public string CompanyName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [StringLength(255), Url]
    public string? Website { get; set; }

    [StringLength(100)]
    public string? Industry { get; set; }

    [StringLength(150), EmailAddress]
    [Display(Name = "Contact Email")]
    public string? ContactEmail { get; set; }

    [StringLength(20), Phone]
    [Display(Name = "Contact Phone")]
    public string? ContactPhone { get; set; }
}
