using Affiliate.Application.Models;

namespace Affiliate.Application.Dtos;

public class WithdrawDto : Withdraw
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public long Points { get; set; }
    public string? Note { get; set; }
}