using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace RendleLabs.Diagnostics.SplitWatchFormat
{
    public class Split
    {
        public Split Parent { get; set; }
        public string Tag { get; set; }
        public string File { get; set; }
        public string Member { get; set; }
        public int Line { get; set; }
        public int Thread { get; set; }
        public bool PoolThread { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public double Elapsed { get; set; }
        public bool Elide { get; set; }
        public Split[] Subs { get; set; }

        public static Split FromXml(XElement element, Split parent)
        {
            var split = new Split
            {
                Parent = parent,
                Tag = element.Attribute("tag")?.Value,
                File = element.Attribute("file")?.Value,
                Member = element.Attribute("member")?.Value,
                Line = int.TryParse(element.Attribute("line")?.Value ?? "0", out var line) ? line : 0,
                Thread = int.TryParse(element.Attribute("thread")?.Value ?? "0", out var thread) ? thread : 0,
                PoolThread = bool.TryParse(element.Attribute("poolThread")?.Value ?? "false", out var poolThread) &&
                             poolThread,
                Start = XmlConvert.ToTimeSpan(element.Attribute("start")?.Value),
                End = XmlConvert.ToTimeSpan(element.Attribute("end")?.Value),
                Elapsed = double.TryParse(element.Attribute("elapsed")?.Value ?? "", out var elapsed) ? elapsed : 0,
                Elide = bool.TryParse(element.Attribute("elide")?.Value ?? "false", out var elide) && elide,
            };
            
            split.Subs = element.Elements("split")
                .Select(e => FromXml(e, split))
                .OrderBy(b => b.Start)
                .ToArray();

            return split;
        }
    }
}