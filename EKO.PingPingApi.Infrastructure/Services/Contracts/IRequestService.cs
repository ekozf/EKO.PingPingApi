﻿using EKO.PingPingApi.Shared.Responses;

namespace EKO.PingPingApi.Infrastructure.Services.Contracts;

/// <summary>
/// Request service interface for making requests to the PingPing website.
/// </summary>
public interface IRequestService
{
    /// <summary>
    /// Log in a user and return the response.
    /// </summary>
    /// <param name="userName">UserName to log in with</param>
    /// <param name="password">Password to use</param>
    /// <returns><see cref="LoginResponse"/> from the server.</returns>
    public Task<LoginResponse> LoginUser(string userName, string password);

    /// <summary>
    /// Get the user purse from the server.
    /// </summary>
    /// <param name="cookie">Authentication cookie of the logged in user</param>
    /// <returns><see cref="PageResponse"/> with the data of the user's purse</returns>
    public Task<PageResponse> GetUserPurse(string cookie);

    /// <summary>
    /// Logs out the user with the given cookie.
    /// </summary>
    /// <param name="cookie">Authentication cookie of the logged in user</param>
    /// <returns>true if the user logged out successfully, otherwise false.</returns>
    public Task<bool> LogOutUser(string cookie);

    /// <summary>
    /// Get the user transactions from the server by date.
    /// </summary>
    /// <param name="cookie">Authentication cookie of the logged in user</param>
    /// <param name="date">Date from which to start getting the transactions from</param>
    /// <returns><see cref="PageResponse"/> from the server.</returns>
    public Task<PageResponse> GetTransactionsByDate(string cookie, DateTime date);

    /// <summary>
    /// Get all the current sessions of the user.
    /// </summary>
    /// <param name="cookie">Authentication cookie of the logged in user</param>
    /// <returns><see cref="PageResponse"/> from the server.</returns>
    public Task<PageResponse> GetAllCurrentSessions(string cookie);

    /// <summary>
    /// Logout a session with the given session id.
    /// </summary>
    /// <param name="cookie">Authentication cookie of the logged in user</param>
    /// <param name="sessionId"></param>
    /// <returns>true if the session was logged out, otherwise false.</returns>
    public Task<bool> LogoutSession(string cookie, string sessionId);

    /// <summary>
    /// Reset the password of the user.
    /// </summary>
    /// <param name="username">Username to reset the password for</param>
    /// <returns>true if a mail was sent</returns>
    public Task<bool> ResetPassword(string username);
}
