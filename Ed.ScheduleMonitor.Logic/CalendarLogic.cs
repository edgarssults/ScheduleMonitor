using Ed.ScheduleMonitor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ed.ScheduleMonitor.Logic
{
    /// <summary>
    /// Calendar management logic.
    /// </summary>
    public class CalendarLogic : ICalendarLogic
    {
        private readonly IDownloadLogic _downloadLogic;
        private readonly IStorageLogic _storageLogic;

        /// <summary>
        /// Calendar management logic.
        /// </summary>
        /// <param name="downloadLogic">Download logic.</param>
        /// <param name="storageLogic">Storage logic.</param>
        public CalendarLogic(
            IDownloadLogic downloadLogic,
            IStorageLogic storageLogic)
        {
            _downloadLogic = downloadLogic;
            _storageLogic = storageLogic;
        }

        /// <summary>
        /// Gets a list of calendar events for a user from storage or the schedule site.
        /// </summary>
        /// <param name="user">The user to retrieve the calendar events for.</param>
        /// <param name="startDate">Start date to get event from.</param>
        /// <param name="endDate">End date to get events to.</param>
        public async Task<List<CalendarEvent>> GetEvents(ApplicationUser user, DateTime startDate, DateTime endDate)
        {
            var html = await _downloadLogic.GetScheduleHtml(user);
            var onlineEvents = _downloadLogic.ParseScheduleHtml(html, user.ScheduleUsername);

            // Compare online and stored events and update storage
            // This should be done in a scheduled job in the future
            foreach (var onlineEvent in onlineEvents)
            {
                var existingEvent = _storageLogic.GetEvent(onlineEvent.UserName, onlineEvent.StartDate);

                if (existingEvent == null)
                {
                    // Add the new event
                    _storageLogic.AddEvent(onlineEvent);
                }
                else if (existingEvent.Name != onlineEvent.Name && existingEvent.Code != onlineEvent.Code)
                {
                    // Replace the existing event if it's been changed
                    _storageLogic.RemoveEvent(existingEvent);
                    _storageLogic.AddEvent(onlineEvent);
                }
            }

            return _storageLogic.GetEvents(user.ScheduleUsername, startDate, endDate);
        }
    }
}
