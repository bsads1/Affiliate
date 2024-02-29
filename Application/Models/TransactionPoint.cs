using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Affiliate.Application.Models;

public class TransactionPoint : ExtendModel
{
    [Description("Identifier for the user's account involved in the transaction.")]
    public Guid UserId { get; set; }

    [Description("Amount of money that was added or removed from the user's account.")]
    public long AmountChanged { get; set; }

    [Description("Points that were added or removed from the user's account.")]
    public long PointsChanged { get; set; }

    [Description("Balance of the user's account at the time of the transaction.")]
    public long AccountBalance { get; set; }

    [Description("Date and time of the transaction.")]
    public DateTime TransactionTimestamp { get; set; }

    [AllowNull]
    [Description("Category of the transaction such as Charge, Redemption, etc.")]
    public string TransactionType { get; set; } = string.Empty;

    [AllowNull]
    [Description("Additional information about the transaction.")]
    public string TransactionDescription { get; set; } = string.Empty;
}