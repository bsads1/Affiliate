using Spark.Library.Database;

namespace Affiliate.Application.Models;

public class MenuItem : ExtendModel
{
    public int Level { get; set; }
    public int ParentId { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
}