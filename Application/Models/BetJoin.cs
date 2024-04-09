namespace Affiliate.Application.Models;

public class BetJoin : ExtendModel
{
    public Guid UserGuid { get; set; }
    public Guid LivestreamGuid { get; set; }
    public Guid BetGuid { get; set; }
    public DateTime JoinDate { get; set; }
    public int BetOnPlayer { get; set; }
    public long PointsBet { get; set; }
    public double RatioBet { get; set; }
    public double RatioWon { get; set; }
}