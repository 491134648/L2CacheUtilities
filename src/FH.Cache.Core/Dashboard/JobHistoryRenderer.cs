﻿// This file is part of FH.Cache.Core.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FH.Cache.Core.Stats;

namespace FH.Cache.Core.Dashboard
{
    public static class JobHistoryRenderer
    {
        private static readonly IDictionary<string, Func<HtmlHelper, IDictionary<string, string>, NonEscapedString>>
            Renderers = new Dictionary<string, Func<HtmlHelper, IDictionary<string, string>, NonEscapedString>>();

        private static readonly IDictionary<string, string> BackgroundStateColors
            = new Dictionary<string, string>();
        private static readonly IDictionary<string, string> ForegroundStateColors
            = new Dictionary<string, string>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static JobHistoryRenderer()
        {
           
            Register(DeletedState.StateName, NullRenderer);
           

           
            BackgroundStateColors.Add(DeletedState.StateName, "#ddd");
          
            ForegroundStateColors.Add(DeletedState.StateName, "#777");
 
        }

        public static void AddBackgroundStateColor(string stateName, string color)
        {
            BackgroundStateColors.Add(stateName, color);
        }

        public static string GetBackgroundStateColor(string stateName)
        {
            if (stateName == null || !BackgroundStateColors.ContainsKey(stateName))
            {
                return "inherit";
            }

            return BackgroundStateColors[stateName];
        }

        public static void AddForegroundStateColor(string stateName, string color)
        {
            ForegroundStateColors.Add(stateName, color);
        }

        public static string GetForegroundStateColor(string stateName)
        {
            if (stateName == null || !ForegroundStateColors.ContainsKey(stateName))
            {
                return "inherit";
            }

            return ForegroundStateColors[stateName];
        }

        public static void Register(string state, Func<HtmlHelper, IDictionary<string, string>, NonEscapedString> renderer)
        {
            if (!Renderers.ContainsKey(state))
            {
                Renderers.Add(state, renderer);
            }
            else
            {
                Renderers[state] = renderer;
            }
        }

        public static bool Exists(string state)
        {
            return Renderers.ContainsKey(state);
        }

        public static NonEscapedString RenderHistory(
            this HtmlHelper helper,
            string state, IDictionary<string, string> properties)
        {
            var renderer = Renderers.ContainsKey(state)
                ? Renderers[state]
                : DefaultRenderer;

            return renderer?.Invoke(helper, properties);
        }

        public static NonEscapedString NullRenderer(HtmlHelper helper, IDictionary<string, string> properties)
        {
            return null;
        }

        public static NonEscapedString DefaultRenderer(HtmlHelper helper, IDictionary<string, string> stateData)
        {
            if (stateData == null || stateData.Count == 0) return null;

            var builder = new StringBuilder();
            builder.Append("<dl class=\"dl-horizontal\">");

            foreach (var item in stateData)
            {
                builder.Append($"<dt>{helper.HtmlEncode(item.Key)}</dt>");
                builder.Append($"<dd>{helper.HtmlEncode(item.Value)}</dd>");
            }

            builder.Append("</dl>");

            return new NonEscapedString(builder.ToString());
        }

        public static NonEscapedString SucceededRenderer(HtmlHelper html, IDictionary<string, string> stateData)
        {
            var builder = new StringBuilder();
            builder.Append("<dl class=\"dl-horizontal\">");

            var itemsAdded = false;

            if (stateData.ContainsKey("Latency"))
            {
                var latency = TimeSpan.FromMilliseconds(long.Parse(stateData["Latency"]));

                builder.Append($"<dt>Latency:</dt><dd>{html.HtmlEncode(html.ToHumanDuration(latency, false))}</dd>");

                itemsAdded = true;
            }

            if (stateData.ContainsKey("PerformanceDuration"))
            {
                var duration = TimeSpan.FromMilliseconds(long.Parse(stateData["PerformanceDuration"]));
                builder.Append($"<dt>Duration:</dt><dd>{html.HtmlEncode(html.ToHumanDuration(duration, false))}</dd>");

                itemsAdded = true;
            }


            if (stateData.ContainsKey("Result") && !String.IsNullOrWhiteSpace(stateData["Result"]))
            {
                var result = stateData["Result"];
                builder.Append($"<dt>Result:</dt><dd>{html.HtmlEncode(result)}</dd>");

                itemsAdded = true;
            }

            builder.Append("</dl>");

            if (!itemsAdded) return null;

            return new NonEscapedString(builder.ToString());
        }


        private static NonEscapedString ProcessingRenderer(HtmlHelper helper, IDictionary<string, string> stateData)
        {
            var builder = new StringBuilder();
            builder.Append("<dl class=\"dl-horizontal\">");

            string serverId = null;

            if (stateData.ContainsKey("ServerId"))
            {
                serverId = stateData["ServerId"];
            }
            else if (stateData.ContainsKey("ServerName"))
            {
                serverId = stateData["ServerName"];
            }

            if (serverId != null)
            {
                builder.Append("<dt>Server:</dt>");
                builder.Append($"<dd>{helper.ServerId(serverId)}</dd>");
            }

            if (stateData.ContainsKey("WorkerId"))
            {
                builder.Append("<dt>Worker:</dt>");
                builder.Append($"<dd>{helper.HtmlEncode(stateData["WorkerId"].Substring(0, 8))}</dd>");
            }
            else if (stateData.ContainsKey("WorkerNumber"))
            {
                builder.Append("<dt>Worker:</dt>");
                builder.Append($"<dd>#{helper.HtmlEncode(stateData["WorkerNumber"])}</dd>");
            }

            builder.Append("</dl>");

            return new NonEscapedString(builder.ToString());
        }

        private static NonEscapedString EnqueuedRenderer(HtmlHelper helper, IDictionary<string, string> stateData)
        {
            return new NonEscapedString(
                $"<dl class=\"dl-horizontal\"><dt>Queue:</dt><dd>{helper.QueueLabel(stateData["Queue"])}</dd></dl>");
        }

       
    }
}
