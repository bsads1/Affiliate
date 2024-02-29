namespace Affiliate.Application.Models;

public class ConfigPage : ExtendModel
{
    public string PageName { get; set; }
    public string Logo { get; set; }
    public string Favicon { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    public string Keywords { get; set; }
    public string AdditionalMeta { get; set; }
    public string CustomCss { get; set; }
    public string CustomJs { get; set; }
}