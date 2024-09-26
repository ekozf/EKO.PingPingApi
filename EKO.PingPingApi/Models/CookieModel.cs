namespace EKO.PingPingApi.Models;

/// <summary>
/// Represents the Cookie from PingPing
/// </summary>
public sealed class CookieModel
{
    /// <summary>
    /// User token
    /// </summary>
    public required string Token { get; init; }
}
