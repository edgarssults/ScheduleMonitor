using Ed.ScheduleMonitor.Web.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Ed.ScheduleMonitor.Web.Helpers
{
    /// <summary>
    /// Email sending helper for ASP.NET identity logic.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly AuthMessageSenderOptions _options;

        /// <summary>
        /// Email sending helper for ASP.NET identity logic.
        /// </summary>
        /// <param name="optionsAccessor">Options.</param>
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        /// <summary>
        /// Send an email.
        /// </summary>
        /// <param name="email">Recipient's email address.</param>
        /// <param name="subject">Message subject.</param>
        /// <param name="message">Message content.</param>
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("noreply@schedulemonitor.net", _options.SendGridUser),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            msg.SetClickTracking(false, false);

            var client = new SendGridClient(_options.SendGridKey);
            return client.SendEmailAsync(msg);
        }
    }
}
