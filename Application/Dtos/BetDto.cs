namespace Affiliate.Application.Dtos;

public class BetDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public DateTime BetDate { get; set; }
    public int BetOnPlayer { get; set; }
    public long PointsBet { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}