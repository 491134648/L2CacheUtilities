using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Linq.Expressions;
using FH.Cache.Core.Dashboard.Resources;
using FH.Cache.Core.Dashboard.Pages;
using System.Globalization;

namespace FH.Cache.Core.Dashboard
{
    public class HtmlHelper
    {
        private static readonly Type DisplayNameType;
        private static readonly Func<object, string> GetDisplayName;

        private readonly RazorPage _page;

        static HtmlHelper()
        {
            try
            {
                DisplayNameType = Type.GetType("System.ComponentModel.DisplayNameAttribute, System.ComponentModel.Primitives");
                if (DisplayNameType == null) return;

                var p = Expression.Parameter(typeof(object));
                var converted = Expression.Convert(p, DisplayNameType);

                GetDisplayName = Expression.Lambda<Func<object, string>>(Expression.Call(converted, "get_DisplayName", null), p).Compile();
            }
            catch
            {
                // Ignoring
            }
        }

        public HtmlHelper(RazorPage page)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));
            _page = page;
        }

        //public NonEscapedString Breadcrumbs(string title,IDictionary<string, string> items)
        //{
        //    if (items == null) throw new ArgumentNullException(nameof(items));
        //    return RenderPartial(new Breadcrumbs(title, items));
        //}

        //public NonEscapedString JobsSidebar()
        //{
        //    return RenderPartial(new SidebarMenu(JobsSidebarMenu.Items));
        //}

        //public NonEscapedString SidebarMenu(IEnumerable<Func<RazorPage, MenuItem>> items)
        //{
        //    if (items == null) throw new ArgumentNullException(nameof(items));
        //    return RenderPartial(new SidebarMenu(items));
        //}

        //public NonEscapedString BlockMetric(DashboardMetric metric)
        //{
        //    if (metric == null) throw new ArgumentNullException(nameof(metric));
        //    return RenderPartial(new BlockMetric(metric));
        //}

        public NonEscapedString InlineMetric(DashboardMetric metric)
        {
            if (metric == null) throw new ArgumentNullException(nameof(metric));
            return RenderPartial(new InlineMetric(metric));
        }

        public NonEscapedString Paginator(Pager pager)
        {
            if (pager == null) throw new ArgumentNullException(nameof(pager));
            return RenderPartial(new Paginator(pager));
        }

        public NonEscapedString PerPageSelector(Pager pager)
        {
            if (pager == null) throw new ArgumentNullException(nameof(pager));
            return RenderPartial(new PerPageSelector(pager));
        }

        public NonEscapedString RenderPartial(RazorPage partialPage)
        {
            partialPage.Assign(_page);
            return new NonEscapedString(partialPage.ToString());
        }

        public NonEscapedString Raw(string value)
        {
            return new NonEscapedString(value);
        }

        public NonEscapedString JobId(string jobId, bool shorten = true)
        {
            Guid guid;
            return new NonEscapedString(HtmlEncode(Guid.TryParse(jobId, out guid)
                ? (shorten ? jobId.Substring(0, 8) + "…" : jobId)
                : $"#{jobId}"));
        }

   
        public NonEscapedString StateLabel(string stateName)
        {
            if (String.IsNullOrWhiteSpace(stateName))
            {
                return Raw($"<em>{HtmlEncode(Strings.Common_NoState)}</em>");
            }

            var style = $"background-color: {JobHistoryRenderer.GetForegroundStateColor(stateName)};";
            return Raw($"<span class=\"label label-default\" style=\"{HtmlEncode(style)}\">{HtmlEncode(stateName)}</span>");
        }
        public NonEscapedString BlockMetric(DashboardMetric metric)
        {
            if (metric == null) throw new ArgumentNullException(nameof(metric));
            return RenderPartial(new BlockMetric(metric));
        }

        public NonEscapedString JobIdLink(string jobId)
        {
            return Raw($"<a href=\"{HtmlEncode(_page.Url.JobDetails(jobId))}\">{JobId(jobId)}</a>");
        }
        public NonEscapedString LocalTime(DateTime value)
        {
            return Raw($"<span data-moment-local=\"{value.ToString(CultureInfo.InvariantCulture)}\">{HtmlEncode(value.ToString(CultureInfo.CurrentUICulture))}</span>");
        }
        public string ToHumanDuration(TimeSpan? duration, bool displaySign = true)
        {
            if (duration == null) return null;

            var builder = new StringBuilder();
            if (displaySign)
            {
                builder.Append(duration.Value.TotalMilliseconds < 0 ? "-" : "+");
            }

            duration = duration.Value.Duration();

            if (duration.Value.Days > 0)
            {
                builder.Append($"{duration.Value.Days}d ");
            }

            if (duration.Value.Hours > 0)
            {
                builder.Append($"{duration.Value.Hours}h ");
            }

            if (duration.Value.Minutes > 0)
            {
                builder.Append($"{duration.Value.Minutes}m ");
            }

            if (duration.Value.TotalHours < 1)
            {
                if (duration.Value.Seconds > 0)
                {
                    builder.Append(duration.Value.Seconds);
                    if (duration.Value.Milliseconds > 0)
                    {
                        builder.Append($".{duration.Value.Milliseconds.ToString().PadLeft(3, '0')}");
                    }

                    builder.Append("s ");
                }
                else
                {
                    if (duration.Value.Milliseconds > 0)
                    {
                        builder.Append($"{duration.Value.Milliseconds}ms ");
                    }
                }
            }

            if (builder.Length <= 1)
            {
                builder.Append(" <1ms ");
            }

            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }

        [Obsolete("This method is unused and will be removed in 2.0.0.")]
        public string FormatProperties(IDictionary<string, string> properties)
        {
            return String.Join(", ", properties.Select(x => $"{x.Key}: \"{x.Value}\""));
        }

        public NonEscapedString QueueLabel(string queue)
        {
            var label = queue != null 
                ? $"<a class=\"text-uppercase\" href=\"{HtmlEncode(_page.Url.Queue(queue))}\">{HtmlEncode(queue)}</a>" 
                : $"<span class=\"label label-danger\"><i>{HtmlEncode(Strings.Common_Unknown)}</i></span>";

            return new NonEscapedString(label);
        }

        public NonEscapedString ServerId(string serverId)
        {
            var parts = serverId.Split(':');
            var shortenedId = parts.Length > 1
                ? String.Join(":", parts.Take(parts.Length - 1))
                : serverId;

            return new NonEscapedString(
                $"<span class=\"labe label-defult text-uppercase\" title=\"{HtmlEncode(serverId)}\">{HtmlEncode(shortenedId)}</span>");
        }

       

        public string HtmlEncode(string text)
        {
            return WebUtility.HtmlEncode(text);
        }
    }
}
