using Ed.ScheduleMonitor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ed.ScheduleMonitor.Logic
{
    /// <summary>
    /// Schedule download and parsing logic.
    /// </summary>
    public class DownloadLogic : IDownloadLogic
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly static Dictionary<string, List<string>> _cookieCache = new Dictionary<string, List<string>>();

        /// <summary>
        /// Schedule download and parsing logic.
        /// </summary>
        /// <param name="clientFactory">HTTP client factory.</param>
        public DownloadLogic(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        /// <summary>
        /// Gets schedule HTML.
        /// </summary>
        /// <param name="user">The user to use for downloading.</param>
        public async Task<string> GetScheduleHtml(ApplicationUser user)
        {
            if (string.IsNullOrEmpty(user.ScheduleUsername)
                || string.IsNullOrEmpty(user.SchedulePassword))
            {
                return null;
            }

            var client = _clientFactory.CreateClient();
            var url = "https://www.mustangs.lv/app/ig/";

            if (!_cookieCache.ContainsKey(user.ScheduleUsername))
            {
                // Log in to get a session cookie
                var loginRequest = new HttpRequestMessage(HttpMethod.Post, url + "?action=auth")
                {
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "login", user.ScheduleUsername },
                        { "pass", user.SchedulePassword },
                    }),
                };
                var loginResponse = await client.SendAsync(loginRequest);
                loginResponse.EnsureSuccessStatusCode();
                _cookieCache.Add(user.ScheduleUsername, loginResponse.Headers.GetValues("Set-Cookie").ToList());
            }

            // Get schedule
            var scheduleRequest = new HttpRequestMessage(HttpMethod.Get, url);
            scheduleRequest.Headers.Add("Set-Cookie", _cookieCache[user.ScheduleUsername]);
            var scheduleResponse = await client.SendAsync(scheduleRequest);
            scheduleResponse.EnsureSuccessStatusCode();
            var schedule = await scheduleResponse.Content.ReadAsStringAsync();

            return schedule;
        }

        /// <summary>
        /// Parses schedule HTML into calendar events.
        /// </summary>
        /// <param name="html">The schedule HTML.</param>
        /// <param name="username">Event user name.</param>
        public List<CalendarEvent> ParseScheduleHtml(string html, string username)
        {
            var entries = new List<CalendarEvent>();
            if (string.IsNullOrEmpty(html))
            {
                return entries;
            }

            var table = Regex.Match(html, @"<table[^>]*>.*?</table>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Value;
            var rowMatches = Regex.Matches(table, @"<tr[^>]*>.*?</tr>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // First row contains the dates
            for (int i = 1; i < rowMatches.Count; i++)
            {
                var row = rowMatches[i].Value;

                // Starting row with today
                var currentDay = DateTime.Now.Date;

                var columnMatches = Regex.Matches(row, @"<td[^>]*>(?<content>.*?)</td>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

                // First column contains the time slot
                for (int j = 1; j < columnMatches.Count; j++)
                {
                    var column = columnMatches[j].Value;

                    if (Regex.IsMatch(column, @"<span[^>]*>\s*x\s*</span>", RegexOptions.IgnoreCase)
                        || Regex.IsMatch(column, @"check_date"))
                    {
                        // No booking, move on
                        currentDay = currentDay.AddDays(1);
                        continue;
                    }

                    var alert = Regex.Match(column, @"onclick=""alert\('(?<name>[^;]+);[^:]*:\s*(?<code>[^;]+);[^:]*:\s*(?<phone>[^\s']+)\s*(\((?<experience>[^\)]+)\))?", RegexOptions.IgnoreCase);
                    var timeSlot = columnMatches[0].Groups["content"].Value;
                    var styleMatch = Regex.Match(column, @"<td[^>]*class=""(?<content>[^""]+)""").Groups["content"].Value;

                    entries.Add(new CalendarEvent
                    {
                        StartDate = new DateTime(
                            currentDay.Year,
                            currentDay.Month,
                            currentDay.Day,
                            int.Parse(timeSlot.Substring(0, 2)),
                            int.Parse(timeSlot.Substring(3, 2)),
                            0),
                        EndDate = new DateTime(
                            currentDay.Year,
                            currentDay.Month,
                            currentDay.Day,
                            int.Parse(timeSlot.Substring(6, 2)),
                            int.Parse(timeSlot.Substring(9, 2)),
                            0),
                        IsRed = styleMatch.Contains("red"),
                        IsGray = styleMatch.Contains("gray2") || (string.IsNullOrWhiteSpace(styleMatch) && !string.IsNullOrEmpty(alert.Groups["name"].Value)),
                        IsGreen = styleMatch.Contains("green"),
                        Name = alert.Groups["name"].Value,
                        Code = alert.Groups["code"].Value,
                        Phone = alert.Groups["phone"].Value,
                        Experience = alert.Groups["experience"].Value,
                        UserName = username,
                    });

                    // Move to next day
                    currentDay = currentDay.AddDays(1);
                }
            }

            return entries.OrderBy(e => e.StartDate).ToList();
        }
    }
}
