using System;
using System.Collections.Generic;

namespace Ed.ScheduleMonitor.Data
{
    /// <summary>
    /// Storage interaction logic.
    /// </summary>
    public interface IStorageLogic
    {
        /// <summary>
        /// Gets stored events for a particular user name beginning after the start date.
        /// </summary>
        /// <param name="username">User name.</param>
        /// <param name="startDate">Start date.</param>
        /// <param name="endDate">End date.</param>
        List<CalendarEvent> GetEvents(string username, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Adds a new event to storage.
        /// </summary>
        /// <param name="evt">Event object.</param>
        void AddEvent(CalendarEvent evt);

        /// <summary>
        /// Removes an event from storage.
        /// </summary>
        /// <param name="evt">Event object.</param>
        void RemoveEvent(CalendarEvent evt);

        /// <summary>
        /// Gets an event if one exists for a particular user at a particular start date (slot).
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="eventStartDate">Event start date (slot).</param>
        CalendarEvent GetEvent(string userName, DateTime eventStartDate);
    }
}
