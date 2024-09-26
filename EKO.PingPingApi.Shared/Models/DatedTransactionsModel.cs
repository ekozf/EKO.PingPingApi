namespace EKO.PingPingApi.Shared.Models;

/// <summary>
/// Class that represents a list of transactions that are dated.
/// </summary>
public sealed class DatedTransactionsModel
{
    public DatedTransactionsModel(DateTime fromDate)
    {
        FromDate = fromDate;
    }

    private List<TransactionModel> _transactions = new();

    /// <summary>
    /// List of transactions.
    /// </summary>
    public List<TransactionModel> Transactions
    {
        get => _transactions;
        set => _transactions = value.OrderByDescending(x => x.Date).ToList();
    }

    /// <summary>
    /// DateTime of when the transactions are from.
    /// </summary>
    public DateTime FromDate { get; }
}
