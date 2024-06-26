using Affiliate.Application.Models;

namespace Affiliate.Application.Dtos;

public class BetResultDto
{
    public bool Success { get; set; }
    public List<string> Messages { get; set; }
    public Bet Data { get; set; }
}