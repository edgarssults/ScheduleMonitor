using Ed.ScheduleMonitor.Data;

namespace Ed.ScheduleMonitor.Logic
{
    /// <summary>
    /// Calendar event extensions.
    /// </summary>
    public static class CalendarEventExtensions
    {
        /// <summary>
        /// Determines whether the calendar event is different from another.
        /// </summary>
        /// <param name="first">The first calendar event.</param>
        /// <param name="second">The second calendar event to compare to.</param>
        public static bool IsDifferentFrom(this CalendarEvent first, CalendarEvent second)
        {
            if (first.Name != second.Name)
            {
                return true;
            }

            if (first.Code != second.Code)
            {
                return true;
            }

            if (first.IsGreen != second.IsGreen)
            {
                return true;
            }
            
            if (first.IsGray != second.IsGray)
            {
                return true;
            }
            
            if (first.IsRed != second.IsRed)
            {
                return true;
            }

            return false;
        }
    }
}
