using Ed.ScheduleMonitor.Web.Models;
using Ed.ScheduleMonitor.Web.Resources;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Ed.ScheduleMonitor.Web.Helpers
{
    /// <summary>
    /// Based on https://cpratt.co/bootstrap-4-responsive-calendar-asp-net-core-taghelper/
    /// </summary>
    [HtmlTargetElement("calendar", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class CalendarTagHelper : TagHelper
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public List<CalendarEventViewModel> Events { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "section";
            output.Attributes.Add("class", "calendar");
            output.Content.SetHtmlContent(GetHtml());
            output.TagMode = TagMode.StartTagAndEndTag;
        }

        private string GetHtml()
        {
            var monthStart = new DateTime(Year, Month, 1);
            var events = Events.OrderBy(d => d.StartDate).GroupBy(e => e.StartDate.Date);
            var days = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

            var html = new XDocument(
                new XElement("div",
                    new XAttribute("class", "container-fluid"),
                    new XElement("header",
                        new XElement("div",
                            new XAttribute("class", "row justify-content-md-center"),
                            new XElement("a",
                                new XAttribute("class", "btn btn-dark align-self-center col-auto mr-auto"),
                                new XAttribute("role", "button"),
                                new XAttribute("title", "Iepriekšējais mēnesis"),
                                new XAttribute("href", $"?year={monthStart.AddMonths(-1).Year}&month={monthStart.AddMonths(-1).Month}"),
                                GetLeftChevron()
                            ),
                            new XElement("h4",
                                new XAttribute("class", "display-4 mb-2 text-center"),
                                $"{Translations.ResourceManager.GetString(monthStart.ToString("MMMM"))} {monthStart.Year}"
                            ),
                            new XElement("a",
                                new XAttribute("class", "btn btn-dark align-self-center col-auto ml-auto"),
                                new XAttribute("role", "button"),
                                new XAttribute("title", "Nākamais mēnesis"),
                                new XAttribute("href", $"?year={monthStart.AddMonths(1).Year}&month={monthStart.AddMonths(1).Month}"),
                                GetRightChevron()
                            )
                        ),
                        new XElement("div",
                            new XAttribute("class", "row d-none d-lg-flex p-1 bg-dark text-white"),
                            days.Select(d =>
                                new XElement("h5",
                                    new XAttribute("class", "col-lg p-1 text-center"),
                                    Translations.ResourceManager.GetString(d.ToString())
                                )
                            )
                        )
                    ),
                    new XElement("div",
                        new XAttribute("class", "row border border-right-0 border-bottom-0"),
                        GetDatesHtml()
                    )
                )
            );

            return html.ToString();

            XElement GetLeftChevron()
            {
                // <svg id="i-chevron-left" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32" width="32" height="32" fill="none" stroke="currentcolor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2">
                // <path d="M20 30 L8 16 20 2" />
                // </svg>

                return new XElement("svg",
                    new XAttribute("id", "i-chevron-left"),
                    new XAttribute("viewBox", "0 0 32 32"),
                    new XAttribute("width", "32"),
                    new XAttribute("height", "32"),
                    new XAttribute("fill", "none"),
                    new XAttribute("stroke", "currentcolor"),
                    new XAttribute("stroke-linecap", "round"),
                    new XAttribute("stroke-linejoin", "round"),
                    new XAttribute("stroke-width", "3"),
                    new XElement("path",
                        new XAttribute("d", "M20 30 L8 16 20 2")
                    )
                );
            }

            XElement GetRightChevron()
            {
                // <svg id="i-chevron-left" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32" width="32" height="32" fill="none" stroke="currentcolor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2">
                // <path d="M12 30 L24 16 12 2" />
                // </svg>

                return new XElement("svg",
                    new XAttribute("id", "i-chevron-left"),
                    new XAttribute("viewBox", "0 0 32 32"),
                    new XAttribute("width", "32"),
                    new XAttribute("height", "32"),
                    new XAttribute("fill", "none"),
                    new XAttribute("stroke", "currentcolor"),
                    new XAttribute("stroke-linecap", "round"),
                    new XAttribute("stroke-linejoin", "round"),
                    new XAttribute("stroke-width", "3"),
                    new XElement("path",
                        new XAttribute("d", "M12 30 L24 16 12 2")
                    )
                );
            }

            IEnumerable<XElement> GetDatesHtml()
            {
                var startDate = monthStart.AddDays(-(int)monthStart.DayOfWeek + 1);
                var dates = Enumerable.Range(0, 42).Select(i => startDate.AddDays(i));

                foreach (var d in dates)
                {
                    // Weekends are ignored
                    if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                    {
                        continue;
                    }

                    if (d.DayOfWeek == DayOfWeek.Monday && d != startDate)
                    {
                        yield return new XElement("div",
                            new XAttribute("class", "w-100"),
                            string.Empty
                        );
                    }

                    string optionalClasses = string.Empty;
                    if (d.Month != monthStart.Month)
                    {
                        optionalClasses = " d-none d-lg-inline-block text-muted";
                    }

                    if (d.Date == DateTime.Now.Date)
                    {
                        optionalClasses += " bg-warning";
                    }
                    else
                    {
                        optionalClasses += " bg-light";
                    }

                    yield return new XElement("div",
                        new XAttribute("class", $"day col-lg p-2 border border-left-0 border-top-0 text-truncate{optionalClasses}"),
                        new XElement("h5",
                            new XAttribute("class", "row align-items-center"),
                            new XElement("span",
                                new XAttribute("class", "date col-1"),
                                d.Day
                            ),
                            new XElement("small",
                                new XAttribute("class", "col d-lg-none text-center text-muted"),
                                d.DayOfWeek.ToString()
                            ),
                            new XElement("span",
                                new XAttribute("class", "col-1"),
                                String.Empty
                            )
                        ),
                        GetEventHtml(d)
                    );
                }
            }

            IEnumerable<XElement> GetEventHtml(DateTime d)
            {
                return events?.SingleOrDefault(e => e.Key == d)?.Select(e =>
                    new XElement("a",
                        new XAttribute("class", GetClasses(e)),
                        GetAttributes(e),
                        GetContent(e)
                    )
                ) ?? new[] {
                    new XElement("p",
                        new XAttribute("class", "d-lg-none"),
                        "No events"
                    )
                };
            }

            string GetContent(CalendarEventViewModel e)
            {
                var components = new List<string>();

                if (!string.IsNullOrEmpty(e.Timeslot))
                {
                    components.Add(e.Timeslot);
                }

                if (!string.IsNullOrEmpty(e.Name))
                {
                    components.Add(e.Name);
                }

                return string.Join(" ", components);
            }

            string GetClasses(CalendarEventViewModel e)
            {
                var sb = new StringBuilder("event d-block p-1 pl-2 pr-2 mb-1 rounded text-truncate small");
                sb.Append($" bg-{(e.IsRed ? "danger" : e.IsGray ? "secondary" : "light")}");
                sb.Append(!e.IsRed && !e.IsGray ? " border text-muted" : " border border-danger text-white");

                return sb.ToString();
            }

            XAttribute[] GetAttributes(CalendarEventViewModel e)
            {
                if (!string.IsNullOrEmpty(e.Name))
                {
                    return new XAttribute[]
                    {
                        new XAttribute("title", e.Name),
                        new XAttribute("data-toggle", "popover"),
                        new XAttribute("data-trigger", "focus"),
                        new XAttribute("tabindex", "0"),
                        new XAttribute("data-content", e.ToString())
                    };
                }

                return Array.Empty<XAttribute>();
            }
        }
    }
}
