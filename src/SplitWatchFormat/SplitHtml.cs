using System;
using System.Linq;

namespace RendleLabs.Diagnostics.SplitWatchFormat
{
    public static class SplitHtml
    {
        public static string Create(Split split, bool collapsed, int i)
        {
            var (leftMargin, rightMargin) = Margins(split);
            var level = CountLevel(split.Parent) - 1;
            return $@"<div class=""split palette-{i%4} level-{level}{(collapsed ? " collapsed" : string.Empty)}"" style=""margin-left:{leftMargin}%;margin-right:{rightMargin}%"">
                          <div class=""split-label""><button class=""toggle-button subs-{split.Subs.Length}"">&gt;</button><span title=""{Title(split)}"">{split.Tag}</span></div>
                          <div class=""spacer"" title=""{Title(split)}"">{split.Elapsed:N}ms</div>
                          <div class=""sub"">
                          {string.Join(Environment.NewLine, split.Subs.Select(s => Create(s, true, i)))}
                          </div>
                          </div>";
        }

        private static (double, double) Margins(Split split)
        {
            if (split.Parent == null) return (0, 0);
            
            var offsetStart = (split.Start - split.Parent.Start).TotalMilliseconds;
            var offsetEnd = (split.Parent.End - split.End).TotalMilliseconds;
            var leftMargin = (offsetStart / split.Parent.Elapsed) * 100;
            var rightMargin = (offsetEnd / split.Parent.Elapsed) * 100;
            return (leftMargin, rightMargin);
        }
        
        private static int CountLevel(Split parent)
        {
            return parent == null ? 0 : 1 + CountLevel(parent.Parent);
        }

        private static string Title(Split split)
        {
            return $"{split.Elapsed:N}ms, [{split.File}[{split.Member}[{split.Line:D}]]]";
        }
    }
}