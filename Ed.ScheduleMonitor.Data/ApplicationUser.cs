using Microsoft.AspNetCore.Identity;

namespace Ed.ScheduleMonitor.Data
{
    /// <summary>
    /// Represents a Schedule Monitor application user.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// User name on the schedule site.
        /// </summary>
        [PersonalData]
        public string ScheduleUsername { get; set; }

        /// <summary>
        /// Password on the schedule site.
        /// </summary>
        [PersonalData]
        public string SchedulePassword { get; set; }
    }
}
