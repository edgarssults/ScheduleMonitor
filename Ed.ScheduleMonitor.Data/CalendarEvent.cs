using System;

namespace Ed.ScheduleMonitor.Data
{
    /// <summary>
    /// Represents a calendar event for a student.
    /// </summary>
    public class CalendarEvent
    {
        /// <summary>
        /// Calendar event identifier, auto-generated.
        /// </summary>
        public long CalendarEventId { get; set; }

        /// <summary>
        /// User name of the user the calendar event is for.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Calendar event start date and time.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Calendar event end date and time.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Name of the student.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Student personal identification code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Student phone number.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Amount of experience the student has at the time of the calendar event.
        /// </summary>
        public string Experience { get; set; }

        /// <summary>
        /// Indicates whether the calendar event is marked red.
        /// </summary>
        public bool IsRed { get; set; }

        /// <summary>
        /// Indicates whether the calendar event is marked gray.
        /// </summary>
        public bool IsGray { get; set; }

        /// <summary>
        /// Indicates whether the calendar event is marked green.
        /// </summary>
        public bool IsGreen { get; set; }
    }
}
