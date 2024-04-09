using Spark.Library.Database;

namespace Affiliate.Application.Models;

public class Livestream : ExtendModel
{
    public string Title { get; set; } = "";
    public string Slug { get; set; } = "";
    public DateTime StartTime { get; set; }
    public DateTime? CloseTime { get; set; }
    public DateTime? AvailableTimeStart { get; set; }
    public DateTime? AvailableTimeEnd { get; set; }
    public string Content { get; set; }
    public string Image { get; set; }
    public string LivestreamInput { get; set; }
    public string Ratio { get; set; } = "";

    public string Player1Name { get; set; } = "";
    public string Player2Name { get; set; } = "";
    public int Winner { get; set; }
    public bool IsShow { get; set; }
    public bool IsEnd { get; set; }
    public bool IsDelete { get; set; }
}