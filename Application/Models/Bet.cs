using Spark.Library.Database;

namespace Affiliate.Application.Models;

public class Bet : ExtendModel
{
    public Guid UserGuid { get; set; }
    public Guid LivestreamGuid { get; set; }
    public int BetOnPlayer { get; set; }
    public DateTime BetDate { get; set; }
    public long PointsBet { get; set; }
}