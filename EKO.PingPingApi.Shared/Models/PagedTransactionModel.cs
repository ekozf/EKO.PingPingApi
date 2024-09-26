namespace EKO.PingPingApi.Shared.Models;

public sealed class PagedTransactionModel
{
    public PagedTransactionModel() { }

    public PagedTransactionModel(int page)
    {
        Page = page;
    }

    public List<TransactionModel> Transactions { get; set; } = new();
    public int Page { get; set; }
    public bool HasReachedEnd { get; set; }
}
