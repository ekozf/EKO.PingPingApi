using EKO.PingPingApi.Infrastructure.Services.Contracts;
using EKO.PingPingApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EKO.PingPingApi.Controllers;

[ApiController]
[Route("api/v1")]
public class PingPingController : ControllerBase
{
    /// <summary>
    /// The service that handles all the logic.
    /// </summary>
    private readonly IPingPingService _service;

    public PingPingController(IPingPingService service)
    {
        _service = service;
    }

    /// <summary>
    /// Logs in the user and returns a cookie.
    /// </summary>
    /// <param name="login">User login details</param>
    /// <returns>Cookie with login token</returns>
    [HttpPost("/")]
    public async Task<IActionResult> DoUserLogin([FromBody] UserLoginModel login)
    {
        // Validate the model
        ArgumentNullException.ThrowIfNull(login, nameof(login));

        if (string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
            return BadRequest(new ErrorModel { Message = "Username or password is empty." });

        // Try to log in the user
        var cookie = await _service.DoUserLogin(login.Username, login.Password);

        // If the cookie is empty, the login was unsuccessful
        if (string.IsNullOrWhiteSpace(cookie))
            return BadRequest(new ErrorModel { Message = "Invalid username or password." });

        // Return the cookie
        return Ok(new CookieModel
        {
            Token = cookie,
        });
    }

    /// <summary>
    /// Logs out the user.
    /// </summary>
    /// <param name="cookie">Cookie to use</param>
    /// <returns>Logs out the user</returns>
    [HttpPost("/logout")]
    public async Task<IActionResult> DoUserLogout([FromBody] CookieModel cookie)
    {
        ArgumentNullException.ThrowIfNull(cookie, nameof(cookie));

        if (string.IsNullOrWhiteSpace(cookie.Token))
            return BadRequest(new ErrorModel { Message = "Cookie is empty." });

        var result = await _service.DoUserLogout(cookie.Token);

        if (!result)
            return BadRequest(new ErrorModel { Message = "Could not log out." });

        return Ok();
    }

    /// <summary>
    /// Gets the user's purse.
    /// </summary>
    /// <param name="cookie">Cookie to use</param>
    /// <returns>Gets the purse for the given cookie</returns>
    [HttpPost("/purse")]
    public async Task<IActionResult> GetUserPurse([FromBody] CookieModel cookie)
    {
        ArgumentNullException.ThrowIfNull(cookie, nameof(cookie));

        if (string.IsNullOrWhiteSpace(cookie.Token))
            return BadRequest(new ErrorModel { Message = "Cookie is empty." });

        var purse = await _service.GetUserPurse(cookie.Token);

        if (purse is null || string.IsNullOrEmpty(purse.UserName))
            return BadRequest(new ErrorModel { Message = "Could not get purse." });

        return Ok(purse);
    }

    /// <summary>
    /// Get all transactions from the user after the given date.
    /// </summary>
    /// <param name="requestedTransactions">Information about the requested transactions</param>
    /// <returns>All found transactions after the given date</returns>
    [HttpPost("/transactions")]
    public async Task<IActionResult> GetTransactionsByDate([FromBody] UserTransactionsModel requestedTransactions)
    {
        ArgumentNullException.ThrowIfNull(requestedTransactions, nameof(requestedTransactions));

        ArgumentNullException.ThrowIfNull(requestedTransactions.Cookie, nameof(requestedTransactions.Cookie));

        if (requestedTransactions.FromDate == default)
            return BadRequest(new ErrorModel { Message = "Date is not set." });

        var transactions = await _service.GetTransactionsByDate(requestedTransactions.Cookie.Token, requestedTransactions.FromDate);

        return Ok(transactions);
    }

    /// <summary>
    /// Get all transactions of the last 30 days.
    /// </summary>
    /// <param name="cookie">Cookie to use</param>
    /// <returns>All transactions from the last 30 days</returns>
    [HttpPost("/recent-transactions")]
    public async Task<IActionResult> GetRecentTransactions([FromBody] CookieModel cookie)
    {
        ArgumentNullException.ThrowIfNull(cookie, nameof(cookie));

        if (string.IsNullOrWhiteSpace(cookie.Token))
            return BadRequest(new ErrorModel { Message = "Cookie is empty." });

        var transactions = await _service.GetRecentTransactionsByDate(cookie.Token);

        return Ok(transactions);
    }

    /// <summary>
    /// Gets the user's logged in sessions.
    /// </summary>
    /// <param name="cookie">Cookie to use</param>
    /// <returns>All user sessions</returns>
    [HttpPost("/sessions")]
    public async Task<IActionResult> GetUserSessions([FromBody] CookieModel cookie)
    {
        ArgumentNullException.ThrowIfNull(cookie, nameof(cookie));

        if (string.IsNullOrWhiteSpace(cookie.Token))
            return BadRequest(new ErrorModel { Message = "Cookie is empty." });

        var sessions = await _service.GetUserSessions(cookie.Token);

        return Ok(sessions);
    }

    /// <summary>
    /// Logs out the user from a specific session.
    /// </summary>
    /// <param name="logout">Session to log out the user from</param>
    /// <returns>True if it succeeds else false</returns>
    [HttpPost("/logout-session")]
    public async Task<IActionResult> LogoutUserSession([FromBody] UserLogoutModel logout)
    {
        ArgumentNullException.ThrowIfNull(logout, nameof(logout));

        ArgumentNullException.ThrowIfNull(logout.Cookie, nameof(logout.Cookie));

        if (string.IsNullOrWhiteSpace(logout.Cookie.Token) || string.IsNullOrWhiteSpace(logout.SessionId))
            return BadRequest(new ErrorModel { Message = "Cookie or session ID is empty." });

        var result = await _service.LogoutSession(logout.Cookie.Token, logout.SessionId);

        return Ok(result);
    }
}
