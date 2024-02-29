using Spark.Library.Database;

namespace Affiliate.Application.Models;

public class PageItem : ExtendModel
{
    public string Type { get; set; }
    public string? Name { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public string Banner { get; set; }
    public string Tags { get; set; }
    public bool IsShow { get; set; }
    public bool IsDelete { get; set; }
}