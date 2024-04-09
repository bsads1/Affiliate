namespace Affiliate.Application.Models;

public class BetPlace : ExtendModel
{
    public Guid UserGuid { get; set; }
    public Guid LivestreamGuid { get; set; }
    public DateTime BetDate { get; set; }
    public long PointsBet { get; set; }
    public int Player { get; set; }
    public int Bet { get; set; }
    public int MoneyBet { get; set; }
    public int MoneyEarn { get; set; }
    public bool IsComplete { get; set; }
}