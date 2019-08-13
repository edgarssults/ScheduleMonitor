using Ed.ScheduleMonitor.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ed.ScheduleMonitor.Logic
{
    /// <summary>
    /// Calendar management logic.
    /// </summary>
    public interface ICalendarLogic
    {
        /// <summary>
        /// Gets a list of calendar events for a user from storage or the schedule site.
        /// </summary>
        /// <param name="user">The user to retrieve the calendar events for.</param>
        /// <param name="startDate">Start date to get event from.</param>
        /// <param name="endDate">End date to get events to.</param>
        Task<List<CalendarEvent>> GetEvents(ApplicationUser user, DateTime startDate, DateTime endDate);
    }
}