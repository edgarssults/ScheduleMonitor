using System;
using System.Collections.Generic;
using System.Linq;

namespace Ed.ScheduleMonitor.Data
{
    /// <summary>
    /// Storage interaction logic.
    /// </summary>
    public class StorageLogic : IStorageLogic
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Storage interaction logic.
        /// </summary>
        /// <param name="context">Database context.</param>
        public StorageLogic(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets stored events for a particular user name beginning after the start date.
        /// </summary>
        /// <param name="username">User name.</param>
        /// <param name="startDate">Start date.</param>
        /// <param name="endDate">End date.</param>
        public List<CalendarEvent> GetEvents(string username, DateTime startDate, DateTime endDate)
        {
            return _context.CalendarEvents
                .Where(e => e.UserName == username && e.StartDate >= startDate && e.EndDate <= endDate)
                .ToList();
        }

        /// <summary>
        /// Adds a new event to storage.
        /// </summary>
        /// <param name="evt">Event object.</param>
        public void AddEvent(CalendarEvent evt)
        {
            _context.CalendarEvents.Add(evt);
            _context.SaveChanges();
        }

        /// <summary>
        /// Removes an event from storage.
        /// </summary>
        /// <param name="evt">Event object.</param>
        public void RemoveEvent(CalendarEvent evt)
        {
            _context.CalendarEvents.Remove(evt);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets an event if one exists for a particular user at a particular start date (slot).
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="eventStartDate">Event start date (slot).</param>
        public CalendarEvent GetEvent(string userName, DateTime eventStartDate)
        {
            return _context.CalendarEvents.FirstOrDefault(e => e.UserName == userName && e.StartDate == eventStartDate);
        }
    }
}
