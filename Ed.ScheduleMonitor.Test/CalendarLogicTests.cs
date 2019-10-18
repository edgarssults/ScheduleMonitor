using Ed.ScheduleMonitor.Data;
using Ed.ScheduleMonitor.Logic;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Ed.ScheduleMonitor.Test
{
    public class CalendarLogicTests
    {
        private readonly Mock<IDownloadLogic> _downloadLogic;
        private readonly Mock<IStorageLogic> _storageLogic;
        private readonly CalendarLogic _target;

        public CalendarLogicTests()
        {
            _downloadLogic = new Mock<IDownloadLogic>();
            _storageLogic = new Mock<IStorageLogic>();

            _target = new CalendarLogic(_downloadLogic.Object, _storageLogic.Object);
        }

        [Fact]
        public async Task GetEvents_NewEventAdded()
        {
            var user = new ApplicationUser
            {
                ScheduleUsername = "test user",
            };
            var startDate = new DateTime(2019, 10, 1);
            var endDate = new DateTime(2019, 10, 31);
            var html = "HTML";

            var storageEvents = new List<CalendarEvent>();
            _storageLogic.Setup(m => m.GetEvents(user.ScheduleUsername, startDate, endDate))
                .Returns(storageEvents);

            _downloadLogic.Setup(m => m.GetScheduleHtml(user))
                .Returns(Task.FromResult(html));
            _downloadLogic.Setup(m => m.ParseScheduleHtml(html, user.ScheduleUsername))
                .Returns(new List<CalendarEvent>
                {
                    new CalendarEvent
                    {
                        CalendarEventId = 1001,
                        Code = "TEST",
                        StartDate = new DateTime(2019, 10, 1, 9, 0, 0),
                        EndDate = new DateTime(2019, 10, 1, 9, 30, 0),
                        Experience = "TEST",
                        Name = "Test Client",
                        Phone = "12345",
                        UserName = user.ScheduleUsername,
                        IsRed = true,
                        IsGray = false,
                    },
                });

            _storageLogic.Setup(m => m.GetEvent(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns<CalendarEvent>(null);
            _storageLogic.Setup(m => m.AddEvent(It.IsAny<CalendarEvent>()))
                .Callback((CalendarEvent evt) =>
                {
                    storageEvents.Add(evt);
                });

            List<CalendarEvent> results = await _target.GetEvents(user, startDate, endDate);

            results.Should().NotBeNullOrEmpty();
            results.Count.Should().Be(1);
            results.First().Name.Should().Be("Test Client");
        }

        [Fact]
        public async Task GetEvents_ExistingEventUpdated()
        {
            var user = new ApplicationUser
            {
                ScheduleUsername = "test user",
            };
            var startDate = new DateTime(2019, 10, 1);
            var endDate = new DateTime(2019, 10, 31);
            var html = "HTML";

            var storageEvents = new List<CalendarEvent>();
            _storageLogic.Setup(m => m.GetEvents(user.ScheduleUsername, startDate, endDate))
                .Returns(storageEvents);

            _downloadLogic.Setup(m => m.GetScheduleHtml(user))
                .Returns(Task.FromResult(html));
            _downloadLogic.Setup(m => m.ParseScheduleHtml(html, user.ScheduleUsername))
                .Returns(new List<CalendarEvent>
                {
                    new CalendarEvent
                    {
                        Code = "TEST",
                        StartDate = new DateTime(2019, 10, 1, 9, 0, 0),
                        EndDate = new DateTime(2019, 10, 1, 9, 30, 0),
                        Experience = "TEST",
                        Name = "Test Client",
                        Phone = "12345",
                        UserName = user.ScheduleUsername,
                        IsRed = true,
                        IsGray = false,
                    },
                });

            _storageLogic.Setup(m => m.GetEvent(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(new CalendarEvent
                {
                    CalendarEventId = 1001,
                    Code = "ORIGINAL", // Different code
                    StartDate = new DateTime(2019, 10, 1, 9, 0, 0),
                    EndDate = new DateTime(2019, 10, 1, 9, 30, 0),
                    Experience = "TEST",
                    Name = "Original Test Client", // Different name
                    Phone = "12345",
                    UserName = user.ScheduleUsername,
                    IsRed = true,
                    IsGray = false,
                });
            _storageLogic.Setup(m => m.AddEvent(It.IsAny<CalendarEvent>()))
                .Callback((CalendarEvent evt) =>
                {
                    storageEvents.Add(evt);
                });
            _storageLogic.Setup(m => m.RemoveEvent(It.IsAny<CalendarEvent>()))
                .Callback((CalendarEvent evt) =>
                {
                    storageEvents.RemoveAll(e => e.CalendarEventId == evt.CalendarEventId);
                });

            List<CalendarEvent> results = await _target.GetEvents(user, startDate, endDate);

            results.Should().NotBeNullOrEmpty();
            results.Count.Should().Be(1);
            results.First().Name.Should().Be("Test Client");
        }

        [Fact]
        public async Task GetEvents_CanceledEventRemoved()
        {
            var user = new ApplicationUser
            {
                ScheduleUsername = "test user",
            };
            var startDate = new DateTime(2019, 10, 1);
            var endDate = new DateTime(2019, 10, 31);
            var html = "HTML";

            var storageEvents = new List<CalendarEvent>
            {
                new CalendarEvent
                {
                    CalendarEventId = 1002,
                    Code = "OLD",
                    StartDate = new DateTime(2019, 10, 1, 9, 0, 0),
                    EndDate = new DateTime(2019, 10, 1, 9, 30, 0),
                    Experience = "TEST",
                    Name = "Old Test Client",
                    Phone = "12345",
                    UserName = user.ScheduleUsername,
                    IsRed = true,
                    IsGray = false,
                },
            };
            _storageLogic.Setup(m => m.GetEvents(user.ScheduleUsername, startDate, endDate))
                .Returns(storageEvents);

            _downloadLogic.Setup(m => m.GetScheduleHtml(user))
                .Returns(Task.FromResult(html));
            _downloadLogic.Setup(m => m.ParseScheduleHtml(html, user.ScheduleUsername))
                .Returns(new List<CalendarEvent>
                {
                    new CalendarEvent
                    {
                        CalendarEventId = 1001,
                        Code = "TEST",
                        StartDate = new DateTime(2019, 10, 1, 10, 0, 0),
                        EndDate = new DateTime(2019, 10, 1, 10, 30, 0),
                        Experience = "TEST",
                        Name = "Test Client",
                        Phone = "12345",
                        UserName = user.ScheduleUsername,
                        IsRed = true,
                        IsGray = false,
                    },
                });

            _storageLogic.Setup(m => m.GetEvent(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns<CalendarEvent>(null);
            _storageLogic.Setup(m => m.AddEvent(It.IsAny<CalendarEvent>()))
                .Callback((CalendarEvent evt) =>
                {
                    storageEvents.Add(evt);
                });

            List<CalendarEvent> results = await _target.GetEvents(user, startDate, endDate);

            results.Should().NotBeNullOrEmpty();
            results.Count.Should().Be(1);
            results.First().Name.Should().Be("Test Client");
            _storageLogic.Verify(m => m.RemoveEvent(It.IsAny<CalendarEvent>()), Times.Once);
        }

        [Fact]
        public async Task GetEvents_NoEvents_ExistingEventsNotRemoved()
        {
            var user = new ApplicationUser
            {
                ScheduleUsername = "test user",
            };
            var startDate = new DateTime(2019, 10, 1);
            var endDate = new DateTime(2019, 10, 31);
            var html = "HTML";

            var storageEvents = new List<CalendarEvent>
            {
                new CalendarEvent
                {
                    CalendarEventId = 1002,
                    Code = "OLD",
                    StartDate = new DateTime(2019, 10, 1, 9, 0, 0),
                    EndDate = new DateTime(2019, 10, 1, 9, 30, 0),
                    Experience = "TEST",
                    Name = "Old Test Client",
                    Phone = "12345",
                    UserName = user.ScheduleUsername,
                    IsRed = true,
                    IsGray = false,
                },
            };
            _storageLogic.Setup(m => m.GetEvents(user.ScheduleUsername, startDate, endDate))
                .Returns(storageEvents);

            _downloadLogic.Setup(m => m.GetScheduleHtml(user))
                .Returns(Task.FromResult(html));
            _downloadLogic.Setup(m => m.ParseScheduleHtml(html, user.ScheduleUsername))
                .Returns(new List<CalendarEvent>());

            List<CalendarEvent> results = await _target.GetEvents(user, startDate, endDate);

            results.Should().NotBeNullOrEmpty();
            results.Count.Should().Be(1);
            results.First().Name.Should().Be("Old Test Client");
            _storageLogic.Verify(m => m.RemoveEvent(It.IsAny<CalendarEvent>()), Times.Never);
        }

        [Fact]
        public async Task GetEvents_OldEventNotRemoved()
        {
            var user = new ApplicationUser
            {
                ScheduleUsername = "test user",
            };
            var startDate = new DateTime(2019, 10, 1);
            var endDate = new DateTime(2019, 10, 31);
            var html = "HTML";

            var storageEvents = new List<CalendarEvent>
            {
                new CalendarEvent
                {
                    CalendarEventId = 1002,
                    Code = "OLD",
                    StartDate = new DateTime(2019, 10, 1, 9, 0, 0),
                    EndDate = new DateTime(2019, 10, 1, 9, 30, 0),
                    Experience = "TEST",
                    Name = "Old Test Client",
                    Phone = "12345",
                    UserName = user.ScheduleUsername,
                    IsRed = true,
                    IsGray = false,
                },
            };
            _storageLogic.Setup(m => m.GetEvents(user.ScheduleUsername, startDate, endDate))
                .Returns(storageEvents);

            _downloadLogic.Setup(m => m.GetScheduleHtml(user))
                .Returns(Task.FromResult(html));
            _downloadLogic.Setup(m => m.ParseScheduleHtml(html, user.ScheduleUsername))
                .Returns(new List<CalendarEvent>
                {
                    new CalendarEvent
                    {
                        CalendarEventId = 1001,
                        Code = "TEST",
                        StartDate = new DateTime(2019, 10, 2, 10, 0, 0),
                        EndDate = new DateTime(2019, 10, 2, 10, 30, 0),
                        Experience = "TEST",
                        Name = "Test Client",
                        Phone = "12345",
                        UserName = user.ScheduleUsername,
                        IsRed = true,
                        IsGray = false,
                    },
                });

            _storageLogic.Setup(m => m.GetEvent(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns<CalendarEvent>(null);
            _storageLogic.Setup(m => m.AddEvent(It.IsAny<CalendarEvent>()))
                .Callback((CalendarEvent evt) =>
                {
                    storageEvents.Add(evt);
                });

            List<CalendarEvent> results = await _target.GetEvents(user, startDate, endDate);

            results.Should().NotBeNullOrEmpty();
            results.Count.Should().Be(2);
            _storageLogic.Verify(m => m.RemoveEvent(It.IsAny<CalendarEvent>()), Times.Never);
        }
    }
}
