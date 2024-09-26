namespace EKO.PingPingApi.Models;

/// <summary>
/// Represents a request to get all transactions from after a given date
/// </summary>
public sealed class UserTransactionsModel
{
    /// <summary>
    /// Cookie from PingPing
    /// </summary>
    public CookieModel Cookie { get; init; } = null!;

    /// <summary>
    /// Date from which to get the transactions from until now
    /// </summary>
    public DateTime FromDate { get; init; }
}
