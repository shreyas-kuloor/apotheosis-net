﻿using Apotheosis.Core.Components.GCPDot.Models;
using HtmlAgilityPack;

namespace Apotheosis.Core.Components.GCPDot.Services;

public static class GcpStatParser
{
    public static IEnumerable<GcpStat> ParseGcpStats(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var statNodes = doc.DocumentNode.SelectNodes("//s");
        var stats = new List<GcpStat>();

        if (statNodes == null) return stats;
        foreach (var node in statNodes)
        {
            if (long.TryParse(node.GetAttributeValue("t", ""), out var time) &&
                double.TryParse(node.InnerText, out var value))
            {
                stats.Add(new GcpStat { Time = time, Value = value });
            }
        }

        return stats;
    }
}