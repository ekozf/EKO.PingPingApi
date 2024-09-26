using EKO.PingPingApi.Infrastructure.Helpers;
using EKO.PingPingApi.Infrastructure.Services.Contracts;
using EKO.PingPingApi.Shared.Models;

namespace EKO.PingPingApi.Infrastructure.Services;

/// <summary>
/// Class that handles everything related to the PingPing service.
/// </summary>
public sealed class PingPingService : IPingPingService
{
    /// <summary>
    /// Request service that handles all the HTTP requests to PingPing.
    /// </summary>
    private readonly IRequestService _request;

    public PingPingService(IRequestService request)
    {
        _request = request;
    }

    public async Task<string> DoUserLogin(string userName, string password)
    {
        var login = await _request.LoginUser(userName, password);

        if (!PageParser.LoginWasValid(login.Page))
            return string.Empty;

        return login.Cookie;
    }

    public async Task<PurseModel?> GetUserPurse(string cookie)
    {
        var purse = await _request.GetUserPurse(cookie!);

        var model = PageParser.ParseUserPurse(purse);

        return model;
    }

    public async Task<bool> DoUserLogout(string cookie)
    {
        return await _request.LogOutUser(cookie!);
    }

    public async Task<DatedTransactionsModel?> GetRecentTransactionsByDate(string cookie)
    {
        return await GetTransactionsByDate(cookie, DateTime.Today.AddMonths(-1));
    }

    public async Task<DatedTransactionsModel?> GetTransactionsByDate(string cookie, DateTime fromDate)
    {
        var transactions = await _request.GetTransactionsByDate(cookie!, fromDate);

        var models = PageParser.ParseTransactionsByDate(transactions, fromDate);

        return models;
    }

    public async Task<SessionsModelList?> GetUserSessions(string cookie)
    {
        var sessions = await _request.GetAllCurrentSessions(cookie!);

        var model = PageParser.ParseUserSessions(sessions);

        return model;
    }

    public async Task<bool> LogoutSession(string cookie, string sessionId)
    {
        return await _request.LogoutSession(cookie, sessionId);
    }
}
