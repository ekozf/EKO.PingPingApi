namespace EKO.PingPingApi.Shared.Responses;

/// <summary>
/// Server response with the authentication cookie.
/// </summary>
public sealed class LoginResponse : PageResponse
{
    /// <summary>
    /// Authentication cookie of the logged in user.
    /// </summary>
    public string Cookie { get; set; }
}
