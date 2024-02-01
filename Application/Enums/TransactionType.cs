using System.ComponentModel;

namespace Affiliate.Application.Enums;

public enum TransactionType
{
    [Description(
        "This transaction type represents when a user uses their points to make a purchase or redeem them for goods or services.")]
    Purchase,

    [Description(
        "This transaction type represents when a user adds more points to their account by purchasing additional points or through other means.")]
    Charge,

    [Description(
        "This transaction type represents when a user redeems their points for a reward or benefit, such as a discount or free item.")]
    Redemption,

    [Description(
        "This transaction type represents when a previously made purchase is reversed, and the points are returned to the user's account.")]
    Refund,

    [Description(
        "This transaction type represents any manual adjustments made to a user's points balance, such as correcting an error or applying a bonus.")]
    Adjustment,

    [Description(
        "This transaction type represents when points expire or are removed from a user's account due to inactivity or a predefined expiration policy.")]
    Expiration,

    [Description(
        "This transaction type represents when a user withdraws money from their account, converting their points into real currency.")]
    Withdrawal
}