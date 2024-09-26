namespace EKO.PingPingApi.Models;

/// <summary>
/// Represents the error model
/// </summary>
public sealed class ErrorModel
{
    /// <summary>
    /// Error message
    /// </summary>
    public required string Message { get; init; }
}
