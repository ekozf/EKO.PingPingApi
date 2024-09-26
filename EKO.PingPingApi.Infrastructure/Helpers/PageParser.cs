using EKO.PingPingApi.Shared.Models;
using EKO.PingPingApi.Shared.Responses;
using System.Text;

namespace EKO.PingPingApi.Infrastructure.Helpers;

/// <summary>
/// Web scraper that parses the HTML page and returns the desired data.
/// </summary>
internal static partial class PageParser
{
    /// <summary>
    /// Checks if the login was valid.
    /// </summary>
    /// <param name="page">Page to scrape</param>
    /// <returns>true if the login has redirected to the main page, false if we got an error.</returns>
    internal static bool LoginWasValid(string page)
    {
        return !page.Contains("Error Invalid Password", StringComparison.InvariantCultureIgnoreCase)
            && !page.Contains("Error Specified account does not exist", StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Scrapes the page and returns the data of the user's purse.
    /// </summary>
    /// <param name="response"><see cref="PageResponse"/> with the retrieved page</param>
    /// <returns>Scraped and parsed purse.</returns>
    internal static PurseModel ParseUserPurse(PageResponse response)
    {
        var page = response.Page;

        return new PurseModel
        {
            UserName = ParseUserName(page),
            Name = ParseName(page),
            Email = ParseEmail(page),
            Purse = ParsePurse(page),
            Balance = ParseBalance(page)
        };
    }

    /// <summary>
    /// Scrapes the page and returns the data of the user's transactions.
    /// </summary>
    /// <param name="response"><see cref="PageResponse"/> with the retrieved page</param>
    /// <param name="page">Page number to get</param>
    /// <returns>List of transactions that the user has made</returns>
    internal static DatedTransactionsModel ParseTransactionsByDate(PageResponse response, DateTime dateFrom)
    {
        // Process loaded page data, remove all tabs
        var data = ProcessTransactionsData(response.Page.Replace("\t", ""));

        var transactions = new DatedTransactionsModel(dateFrom)
        {
            Transactions = data
        };

        return transactions;
    }

    /// <summary>
    /// Parses the page and returns the data of the user's sessions.
    /// </summary>
    /// <param name="responsePage"><see cref="ReadOnlySpan{Char}"/> of the entire page</param>
    /// <returns>All previously done transactions</returns>
    private static List<TransactionModel> ProcessTransactionsData(ReadOnlySpan<char> responsePage)
    {
        var transactions = new List<TransactionModel>();

        // Get all the transactions by even and odd rows in the table
        var parsedOddRows = ParseAllRows("<tr class=\"\" >", responsePage);
        var parsedEvenRows = ParseAllRows("<tr class=\"even \" >", responsePage);

        // Split the rows into individual columns to be parsed later, removing the last </td> tag since it's not needed and is on another line
        var oddRows = parsedOddRows.Split('\n').Skip(1).Where(x => x != "</td>").ToList();
        var evenRows = parsedEvenRows.Split('\n').Skip(1).Where(x => x != "</td>").ToList();

        // Process the columns and add them to the transactions list
        var oddColumns = ProcessColumns(oddRows);
        var evenColumns = ProcessColumns(evenRows);

        transactions.AddRange(oddColumns);
        transactions.AddRange(evenColumns);

        // Order the transactions by date
        transactions = transactions.OrderByDescending(x => x.Date).ToList();

        return transactions;
    }

    /// <summary>
    /// Processes the columns of the transactions and returns them as a list.
    /// </summary>
    /// <param name="rows">Individual rows of the transaction details</param>
    /// <returns>List of all processed column data</returns>
    private static List<TransactionModel> ProcessColumns(IReadOnlyList<string> rows)
    {
        var transactions = new List<TransactionModel>();

        for (int i = 0; i < rows.Count; i += 4)
        {
            // Get the columns for the transaction
            var columnDate = rows[i].AsSpan();
            var columnLocation = rows[i + 1].AsSpan();
            var columnDescription = rows[i + 2].AsSpan();
            var columnPrice = rows[i + 3].AsSpan();

            // 19 is the length of the date string (dd-MM-yyyy HH:mm:ss)
            var date = columnDate.Slice(columnDate.IndexOf("<td>") + "<td>".Length, 19).ToString();
            
            var location = columnLocation.Slice(columnLocation.IndexOf("<td>") + "<td>".Length, columnLocation.IndexOf("</td>") - columnLocation.IndexOf("<td>") - "<td>".Length).ToString();
            
            var description = columnDescription.Slice(columnDescription.IndexOf("<td style=\"word-wrap:break-all;\">") + "<td style=\"word-wrap:break-all;\">".Length, columnDescription.IndexOf("</td>") - columnDescription.IndexOf("<td style=\"word-wrap:break-all;\">") - "<td style=\"word-wrap:break-all;\">".Length).ToString();
            
            var price = columnPrice.Slice(columnPrice.IndexOf("<td class=\"\">") + "<td class=\"\">".Length).ToString().Replace("g-green\">", "");

            transactions.Add(new TransactionModel
            {
                Date = ConvertStringDateToDateTime(date),
                Description = description,
                Price = ConvertTransactionPriceToDouble(price),
                Location = location
            });
        }

        return transactions;
    }

    /// <summary>
    /// Parses the page and returns the individual transactions grouped by 4 columns.
    /// </summary>
    /// <param name="rowHeader">Row HTML tag to search for</param>
    /// <param name="responsePage">The page to search on</param>
    /// <returns>All individual transactions, each 4 lines is a transaction (Date, Location, Description, Price)</returns>
    private static string ParseAllRows(string rowHeader, ReadOnlySpan<char> responsePage) 
    {
        // Get the offset of the table row
        int offset = rowHeader.Length;

        // Find the first index of the row header
        var index = responsePage.IndexOf(rowHeader);

        var builder = new StringBuilder();

        while (index != -1)
        {
            // Skip the row header
            var start = index + rowHeader.Length;

            // Find the end of the row
            var end = responsePage.Slice(start).IndexOf("</tr>");

            // Get all the details for the transaction
            builder.Append(responsePage.Slice(start, end));

            // Find the next row header
            var next = responsePage.Slice(start + offset).IndexOf(rowHeader);

            // If we can't find the next row header, we have reached the end
            if (next == -1)
            {
                break;
            }

            // Continue
            index = next + start;
        }

        // Remove the row header
        return builder.Replace(rowHeader, "").ToString();
    }

    /// <summary>
    /// Parses the page and returns the data of the user's sessions.
    /// </summary>
    /// <param name="sessions"><see cref="PageResponse"/> with the retrieved page</param>
    /// <returns><see cref="SessionsModelList"/> that has all the current user sessions.</returns>
    internal static SessionsModelList ParseUserSessions(PageResponse sessions)
    {
        var responsePage = sessions.Page.Replace("\t", "").Split('\n');

        // DateTimes list has an empty entry somewhere, get its index
        var dateTimes = ParseSessionDateTimes(responsePage);

        var emptyIndex = dateTimes.FindIndex(x => x?.Length == 0);

        // Clear the empty entry
        var userAgents = ParseSessionUserAgents(responsePage);

        // Remove the empty entry
        userAgents[emptyIndex] = string.Empty;

        // Add a empty entry to the list so we can enumerate them together
        var sessionIds = ParseSessionIds(responsePage);

        sessionIds.Insert(emptyIndex, string.Empty);

        // Combine the data into one list so we can enumerate them together
        var dataZip = dateTimes
                        .Zip(userAgents, (Date, UserAgent) => new { Date, UserAgent })
                        .Zip(sessionIds, (x, SessionId) => new { x.Date, x.UserAgent, SessionId });

        var sessionModelList = new SessionsModelList();

        foreach (var entry in dataZip)
        {
            // Skip the empty entry
            if (string.IsNullOrWhiteSpace(entry.Date))
                continue;

            sessionModelList.Sessions.Add(new SessionsModel
            {
                LastActiveDate = ConvertStringDateToDateTime(entry.Date, usesDashes: false),
                UserAgent = UserAgentParser.GetUserAgent(entry.UserAgent) ?? new ReadableUserAgent(),
                SessionId = entry.SessionId
            });
        }

        return sessionModelList;
    }
}
