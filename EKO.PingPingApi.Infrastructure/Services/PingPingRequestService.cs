﻿using EKO.PingPingApi.Infrastructure.Services.Contracts;
using EKO.PingPingApi.Shared.Responses;
using System.Net;
using static EKO.PingPingApi.Shared.AppConsts;

namespace EKO.PingPingApi.Infrastructure.Services;

/// <summary>
/// Class that handles all requests to the PingPing website.
/// </summary>
public sealed class PingPingRequestService : IRequestService
{
    /// <summary>
    /// HttpClient that will be used to send requests.
    /// </summary>
    private readonly HttpClient _httpClient;

    public PingPingRequestService()
    {
        // We need to disable cookies and redirects, because the website will redirect us to the login page if we don't have a valid cookie.
        // We will also get redirected once we log out.
        var clientHandler = new HttpClientHandler
        {
            UseCookies = false,
            AllowAutoRedirect = false,
        };

        // New instance of HttpClient with the custom handler.
        _httpClient = new HttpClient(clientHandler);
    }

    public async Task<LoginResponse> LoginUser(string userName, string password)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(BASE_URL),
            Headers =
            {
                 // We need to set a user agent, otherwise the website will return an error.
                 { "user-agent", USER_AGENT },
            },
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "Username", userName },
                { "Password", password },
                { "RememberMe", "on" },
            }),
        };

        using var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        // We need to get the cookie from the response, otherwise we won't be able to access the user's purse.
        var loginCookie = response
                            .Headers
                            .NonValidated
                            .FirstOrDefault(x => x.Key == "Set-Cookie")
                            .Value
                            .FirstOrDefault(x => x.Contains("__Secure-myNetpay"))?
                            .Split(';')
                            .FirstOrDefault();

        // Return the cookie and the page content.
        return new LoginResponse
        {
            Cookie = loginCookie!,
            Page = await response.Content.ReadAsStringAsync(),
        };
    }

    public async Task<PageResponse> GetUserPurse(string cookie)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(BASE_URL + "?view=profile"),
            Headers =
            {
                { "cookie", cookie },
                { "user-agent", USER_AGENT },
                { "accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7" },
                { "cache-control", "no-cache" },
                { "pragma", "no-cache" },
            },
        };

        using var response = await _httpClient.SendAsync(request);

        return new PageResponse
        {
            Page = await response.Content.ReadAsStringAsync(),
        };
    }

    public async Task<bool> LogOutUser(string cookie)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(BASE_URL + "?logout"),
            Headers =
            {
                { "user-agent", USER_AGENT },
                { "cookie", cookie },
                { "cache-control", "no-cache" },
                { "pragma", "no-cache" },
            },
        };

        using var response = await _httpClient.SendAsync(request);

        // If the user is logged out, the server will redirect to the login page
        return response.StatusCode == HttpStatusCode.MovedPermanently;
    }

    public async Task<PageResponse> GetTransactionsByDate(string cookie, DateTime date)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(BASE_URL + "?view=transaction"),
            Headers =
            {
                { "user-agent", USER_AGENT },
                { "cookie", cookie },
                { "accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7" },
            },
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "StartDate", date.ToString("dd-MM-yyyy") },
                { "EndDate", DateTime.Today.ToString("dd-MM-yyyy") },
                { "search", "" },
                { "s", "" },
            }),
        };

        using var response = await _httpClient.SendAsync(request);

        return new PageResponse
        {
            Page = await response.Content.ReadAsStringAsync(),
        };
    }

    public async Task<PageResponse> GetAllCurrentSessions(string cookie)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(BASE_URL + "?view=rememberme"),
            Headers =
            {
                { "user-agent", USER_AGENT },
                { "cookie", cookie },
                { "cache-control", "no-cache" },
                { "pragma", "no-cache" },
            },
        };

        using var response = await _httpClient.SendAsync(request);

        return new PageResponse
        {
            Page = await response.Content.ReadAsStringAsync(),
        };
    }

    public async Task<bool> LogoutSession(string cookie, string sessionId)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(BASE_URL + "?view=rememberme"),
            Headers =
            {
                { "user-agent", USER_AGENT },
                { "cookie", cookie },
                { "cache-control", "no-cache" },
                { "pragma", "no-cache" },
            },
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "delete", sessionId },
            }),
        };

        using var response = await _httpClient.SendAsync(request);

        return true;
    }

    public async Task<bool> ResetPassword(string username)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(BASE_URL + "?view=resetpassword"),
            Headers =
            {
                { "user-agent", USER_AGENT },
                { "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7" },
            },
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "type", "user" },
                { "Username", username },
                { "reset", "Wachtwoord opnieuw instellen" },
            }),
        };

        using var response = await _httpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }
}
