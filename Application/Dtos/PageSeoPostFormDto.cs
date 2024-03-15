namespace Affiliate.Application.Dtos;

public class PageSeoPostFormDto : PageSeo
{ 
    public IFormFile? File { get; set; }
    public bool HasImage { get; set; }
    public string Error { get; set; }
}