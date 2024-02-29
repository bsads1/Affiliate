namespace Affiliate.Application.Dtos;

public class ConfigPostFormDto
{
    public string? PageName { get; set; }= default!;
    public string? Logo { get; set; }= default!;
    public string? Favicon { get; set; }= default!;
    public string? Description { get; set; }= default!;
    public string? Author { get; set; }= default!;
    public string? Keywords { get; set; }= default!;
    public string? AdditionalMeta { get; set; }= default!;
    public string? CustomCss { get; set; }= default!;
    public string? CustomJs { get; set; }= default!; 

    public IFormFile? FileLogo { get; set; } = default!;
    public IFormFile? FileFavicon { get; set; } = default!;
    public bool LogoHasData { get; set; }
    public bool FaviconHasData { get; set; }
    public Guid CreatedBy { get; set; }
}