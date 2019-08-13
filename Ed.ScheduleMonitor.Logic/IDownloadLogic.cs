using Ed.ScheduleMonitor.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ed.ScheduleMonitor.Logic
{
    /// <summary>
    /// Schedule download and parsing logic.
    /// </summary>
    public interface IDownloadLogic
    {
        /// <summary>
        /// Gets schedule HTML.
        /// </summary>
        /// <param name="user">The user to use for downloading.</param>
        Task<string> GetScheduleHtml(ApplicationUser user);

        /// <summary>
        /// Parses schedule HTML into calendar events.
        /// </summary>
        /// <param name="html">The schedule HTML.</param>
        /// <param name="username">Event user name.</param>
        List<CalendarEvent> ParseScheduleHtml(string html, string username);
    }
}
