using Microsoft.Extensions.Primitives;

namespace Affiliate.Application.Dtos;

public class PagePostFormDto
{
    public int? Id { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string? Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Image { get; set; } = default!;
    public string Banner { get; set; } = default!;
    public string[] Tags { get; set; } = default!;
    public bool? IsShow { get; set; } = default!;

    public IFormFile? File { get; set; } = default!;
    public IFormFile? FileBanner { get; set; } = default!;
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool HasImage { get; set; }
    public bool HasBanner { get; set; }
}