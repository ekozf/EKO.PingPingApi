namespace EKO.PingPingApi.Shared.Models;

public sealed class PageTransactionListModel
{
    public List<PagedTransactionModel> PagedTransactions { get; set; } = new List<PagedTransactionModel>();
}
