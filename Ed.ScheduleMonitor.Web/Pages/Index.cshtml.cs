using Ed.ScheduleMonitor.Data;
using Ed.ScheduleMonitor.Logic;
using Ed.ScheduleMonitor.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ed.ScheduleMonitor.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICalendarLogic _calendarLogic;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            ICalendarLogic calendarLogic)
        {
            _userManager = userManager;
            _calendarLogic = calendarLogic;
        }

        public List<CalendarEventViewModel> CalendarEvents { get; set; }

        public DateTime CurrentDate { get; set; }

        public async Task OnGet([FromQuery] int? year, [FromQuery] int? month)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Determine the current date which will be used to choose the month to show in the calendar
                if (year.HasValue && month.HasValue)
                {
                    CurrentDate = new DateTime(year.Value, month.Value, 1);
                }
                else
                {
                    CurrentDate = DateTime.Now;
                }

                var monthStart = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
                var startDate = monthStart.AddDays(-(int)monthStart.DayOfWeek + 1);
                var endDate = startDate.AddMonths(1).AddDays(14).AddHours(23).AddMinutes(59);

                var user = await _userManager.GetUserAsync(User);
                var events = await _calendarLogic.GetEvents(user, startDate, endDate);

                CalendarEvents = events.Select(e => new CalendarEventViewModel
                {
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Name = e.Name,
                    Phone = e.Phone,
                    Experience = e.Experience,
                    IsRed = e.IsRed,
                    IsGray = e.IsGray,
                }).ToList();
            }
        }
    }
}
