using Ed.ScheduleMonitor.Logic;
using FluentAssertions;
using Moq;
using System.IO;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace Ed.ScheduleMonitor.Test
{
    public class DownloadLogicTests
    {
        public readonly Mock<IHttpClientFactory> _httpClientFactory;
        public readonly DownloadLogic _target;

        public DownloadLogicTests()
        {
            _httpClientFactory = new Mock<IHttpClientFactory>();

            _target = new DownloadLogic(_httpClientFactory.Object);
        }

        [Fact]
        public void ParseScheduleHtml_ShouldWorkWithNewHtml()
        {
            var html = File.ReadAllText(@"Resources\NewSchedule.html");
            var username = "Tester";

            var results = _target.ParseScheduleHtml(html, username);

            results.Count.Should().Be(280);
            results.Where(r => string.IsNullOrEmpty(r.Name)).Count().Should().Be(221);
            results.Where(r => !string.IsNullOrEmpty(r.Name)).Count().Should().Be(59);
            results.Where(r => r.IsGreen).Count().Should().Be(82);
            results.Where(r => r.IsRed).Count().Should().Be(58);
            results.Where(r => r.IsGray).Count().Should().Be(81);
        }
    }
}
