using System;

namespace Ed.ScheduleMonitor.Web.Models
{
    /// <summary>
    /// Represents a calendar event for a student when displayed on the site.
    /// </summary>
    public class CalendarEventViewModel
    {
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
        /// Calendar event time slot.
        /// </summary>
        public string Timeslot
        {
            get
            {
                return $"{StartDate:HH:mm}-{EndDate:HH:mm}";
            }
        }

        /// <summary>
        /// Creates a string representation of the calendar event.
        /// </summary>
        public override string ToString()
        {
            return $"{Timeslot}, {Phone}, {Experience}";
        }
    }
}
