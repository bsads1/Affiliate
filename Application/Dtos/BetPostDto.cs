namespace Affiliate.Application.Dtos;

public class BetPostDto
{
    public int Live { get; set; }
    public int Bet { get; set; }
    public Guid CreateBy { get; set; }
    public Guid LiveGuid { get; set; }
}