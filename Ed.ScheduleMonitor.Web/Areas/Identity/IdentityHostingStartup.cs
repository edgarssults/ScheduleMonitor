using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Ed.ScheduleMonitor.Web.Areas.Identity.IdentityHostingStartup))]
namespace Ed.ScheduleMonitor.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}