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
            string html = await _downloadLogic.GetScheduleHtml(user);
            List<CalendarEvent> onlineEvents = _downloadLogic.ParseScheduleHtml(html, user.ScheduleUsername);

            // Update storage with new and changed events
            // TODO: This should be done in a scheduled job in the future
            foreach (CalendarEvent onlineEvent in onlineEvents)
            {
                CalendarEvent storageEvent = _storageLogic.GetEvent(onlineEvent.UserName, onlineEvent.StartDate);

                if (storageEvent == null)
                {
                    // Add the new event
                    _storageLogic.AddEvent(onlineEvent);
                }
                else if (storageEvent.Name != onlineEvent.Name && storageEvent.Code != onlineEvent.Code)
                {
                    // Replace the existing event if it's been changed
                    _storageLogic.RemoveEvent(storageEvent);
                    _storageLogic.AddEvent(onlineEvent);
                }
            }

            List<CalendarEvent> storageEvents = _storageLogic.GetEvents(user.ScheduleUsername, startDate, endDate);

            if (onlineEvents.Any())
            {
                // Determine which calendar events have been canceled and should be removed
                // TODO: This should be done in a scheduled job in the future
                var storageEventsToRemove = new List<CalendarEvent>();
                foreach (CalendarEvent storageEvent in storageEvents)
                {
                    CalendarEvent onlineEvent = onlineEvents.FirstOrDefault(e => e.StartDate == storageEvent.StartDate);

                    if (onlineEvent == null)
                    {
                        // Add the event for removal
                        storageEventsToRemove.Add(storageEvent);
                    }
                }

                // Remove the canceled events
                foreach (CalendarEvent storageEvent in storageEventsToRemove)
                {
                    _storageLogic.RemoveEvent(storageEvent);
                    storageEvents.RemoveAll(e => e.StartDate == storageEvent.StartDate);
                }
            }

            return storageEvents;
        }
    }
}
