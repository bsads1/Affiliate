using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Affiliate.Application.Models;

public class PageSeo : ExtendModel
{
    [Required]
    [MaxLength(10)]
    public string PageType { get; set; }

    [Required]
    [MaxLength(10)]
    public string PageId { get; set; }

    [MaxLength(100)]
    public string PageRoute { get; set; } = "";

    [Required]
    [MaxLength(200)]
    [Description("Should be less than 70 characters")]
    public string Title { get; set; } = "";

    [MaxLength(1000)]
    [Description("Should be less than 160 characters")]
    public string Description { get; set; } = "";

    [MaxLength(100)]
    [Description("Should be less than 100 characters")]
    public string Keywords { get; set; } = "";

    [MaxLength(255)]
    public string Image { get; set; } = "";

    [MaxLength(255)]
    [Url]
    public string Canonical { get; set; } = "";

    [MaxLength(30)]
    public string Robots { get; set; } = "";
}