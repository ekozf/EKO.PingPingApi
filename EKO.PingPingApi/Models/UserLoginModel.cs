namespace EKO.PingPingApi.Models;

/// <summary>
/// Represents the user login model
/// </summary>
public sealed class UserLoginModel
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
