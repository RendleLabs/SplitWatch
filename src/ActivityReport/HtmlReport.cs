using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReport
{
    public class HtmlReport
    {
        private readonly IList<Entry> _entries;
        private readonly DateTimeOffset _start;
        private readonly DateTimeOffset _end;

        public HtmlReport(IEnumerable<Entry> entries)
        {
            _entries = entries.Where(e => e.StartTime != default && e.Duration != TimeSpan.Zero).OrderBy(e => e.StartTime).ToArray();
            _start = _entries.Min(e => e.StartTime);
            _end = _entries.Max(e => e.StartTime + e.Duration);
        }

        public async Task Write(TextWriter writer)
        {
            foreach (var (entry, index) in _entries.Select((e,i) => (e,i)))
            {
                await Write(writer, entry, true, index);
            }
        }

        private async Task Write(TextWriter writer, Entry entry, bool collapsed, int index)
        {
            var (leftMargin, rightMargin) = Margins(entry);
            var level = CountLevel(entry.Parent);
            await writer.WriteLineAsync($@"<div class=""split palette-{index%4} level-{level}{(collapsed ? " collapsed" : string.Empty)}"" style=""margin-left:{leftMargin}%;margin-right:{rightMargin}%"">
                          <div class=""split-label""><button class=""toggle-button subs-{entry.Children.Count}"">&gt;</button><span title=""{Title(entry)}"">{entry.Operation}</span></div>
                          <div class=""spacer"" title=""{Title(entry)}"">{entry.Duration.TotalMilliseconds:N}ms</div>");
            if (entry.Children.Any())
            {
                await writer.WriteLineAsync(@"<div class=""sub"">");
                foreach (var child in entry.Children)
                {
                    await Write(writer, child, true, index);
                }
                await writer.WriteLineAsync("</div>");
            }
            await writer.WriteLineAsync("</div");
        }

        private (double, double) Margins(Entry entry)
        {
            var (parentStart, parentEnd, parentDuration) = ParentValues(entry.Parent);
            var offsetStart = (entry.StartTime - parentStart).TotalMilliseconds;
            var offsetEnd = (parentEnd - (entry.StartTime + entry.Duration)).TotalMilliseconds;
            var leftMargin = (offsetStart / parentDuration) * 100;
            var rightMargin = (offsetEnd / parentDuration) * 100;
            return (leftMargin, rightMargin);
        }

        private (DateTimeOffset, DateTimeOffset, double) ParentValues(Entry parent)
        {
            if (parent == null) return (_start, _end, (_end - _start).TotalMilliseconds);
            var end = parent.StartTime + parent.Duration;
            return (parent.StartTime, end, (end - parent.StartTime).TotalMilliseconds);
        }
        
        private static int CountLevel(Entry parent)
        {
            return parent == null ? 0 : 1 + CountLevel(parent.Parent);
        }
        private static string Title(Entry split)
        {
            return $"{split.Duration.TotalMilliseconds:N}ms, [{split.Source}][{split.Operation}]";
        }
    }
}