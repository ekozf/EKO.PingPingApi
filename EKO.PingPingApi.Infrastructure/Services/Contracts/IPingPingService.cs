using EKO.PingPingApi.Shared.Models;

namespace EKO.PingPingApi.Infrastructure.Services.Contracts;

/// <summary>
/// Service that handles all the communication with the PingPing and user login.
/// </summary>
public interface IPingPingService
{
    /// <summary>
    /// Log in the user with the given credentials.
    /// </summary>
    /// <param name="userName">UserName of the user</param>
    /// <param name="password">Password of the user</param>
    public Task<string> DoUserLogin(string userName, string password);

    /// <summary>
    /// Get the user's purse from the API.
    /// </summary>
    /// <returns>Returns the <see cref="PurseModel"/> otherwise returns null.</returns>
    public Task<PurseModel?> GetUserPurse(string cookie);

    /// <summary>
    /// Log out the user.
    /// </summary>
    public Task<bool> DoUserLogout(string cookie);

    /// <summary>
    /// Get the user's recent transactions from the API.
    /// </summary>
    /// <returns>Returns an IEnumerable of <see cref="DatedTransactionsModel"/> otherwise returns null.</returns>
    public Task<DatedTransactionsModel?> GetRecentTransactionsByDate(string cookie);

    /// <summary>
    /// Get a page from the user's transactions from the API.
    /// </summary>
    /// <param name="fromDate">Transaction from this date until today</param>
    /// <returns>Returns an IEnumerable of <see cref="DatedTransactionsModel"/> otherwise returns null.</returns>
    public Task<DatedTransactionsModel?> GetTransactionsByDate(string cookie, DateTime fromDate);

    /// <summary>
    /// Gets the user's sessions from the API.
    /// </summary>
    /// <returns>Returns the <see cref="SessionsModel"/> otherwise returns null.</returns>
    public Task<SessionsModelList?> GetUserSessions(string cookie);

    /// <summary>
    /// Log out a session from the API.
    /// </summary>
    /// <param name="sessionId">Session ID to log out.</param>
    /// <returns>true if the logout was successful, otherwise false.</returns>
    public Task<bool> LogoutSession(string cookie, string sessionId);

    /// <summary>
    /// Reset the password of the user.
    /// </summary>
    /// <param name="username">Username to reset the password for</param>
    /// <returns>true if the reset was successful</returns>
    public Task<bool> ResetPassword(string username);
}
