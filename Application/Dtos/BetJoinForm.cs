namespace Affiliate.Application.Dtos;

public class BetJoinForm
{
    public Guid BetGuid { get; set; }
    public long Points { get; set; }
    public Guid UserId { get; set; }
}