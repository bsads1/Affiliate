namespace Affiliate.Application.Models;

public class Withdraw : ExtendModel
{
    public Guid UserGuid { get; set; }
    public long Amount { get; set; }
    public long Points { get; set; }
    public string BankName { get; set; }
    public string AccountNumber { get; set; }
    public string Status { get; set; }
    public string Note { get; set; }
}