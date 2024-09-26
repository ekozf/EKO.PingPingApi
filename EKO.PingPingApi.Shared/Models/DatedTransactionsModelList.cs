namespace EKO.PingPingApi.Shared.Models;

public sealed class DatedTransactionsModelList
{
    private List<DatedTransactionsModel> _datedTransactions = new List<DatedTransactionsModel>();

    public List<DatedTransactionsModel> DatedTransactions
    {
        get => _datedTransactions;
        set => _datedTransactions = value.OrderByDescending(x => x.FromDate).ToList();
    }
}
