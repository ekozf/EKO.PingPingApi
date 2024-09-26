namespace EKO.PingPingApi.Shared.Models;
public sealed class TransactionModel
{
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
}
