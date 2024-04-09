namespace Affiliate.Application.Models;

public class OrderPoint : ExtendModel
{
    public string OrderUid { get; set; }
    public Guid UserGuid { get; set; }
    public long Amount { get; set; }
    public long Points { get; set; }
    public long ExchangeRate { get; set; }
    public string BankName { get; set; }
    public string AccountBankName { get; set; }
    public string BankNumber { get; set; }
    public string Status { get; set; }
    public bool IsDelete { get; set; }
}