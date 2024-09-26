namespace EKO.PingPingApi.Models;

/// <summary>
/// Represents the user logout model
/// </summary>
public sealed class UserLogoutModel
{
    public CookieModel Cookie { get; init; } = null!;
    public string SessionId { get; init; } = string.Empty;
}
