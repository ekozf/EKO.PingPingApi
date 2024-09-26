namespace EKO.PingPingApi.Shared.Models;

public sealed class SessionsModel
{
    public DateTime LastActiveDate { get; set; }
    public ReadableUserAgent UserAgent { get; set; }
    public string SessionId { get; set; } = string.Empty;
}
