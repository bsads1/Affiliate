using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Affiliate.Application.Models;

public class TransactionPoint : ExtendModel
{
    public Guid UserId { get; set; }
    public long Points { get; set; }
    public DateTime TransactionDate { get; set; }
    [AllowNull]
    public string TransactionType { get; set; }
    [AllowNull]
    public string Description { get; set; }
}