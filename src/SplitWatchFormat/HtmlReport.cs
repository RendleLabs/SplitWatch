using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RendleLabs.Diagnostics.SplitWatchFormat
{
    public static class HtmlReport
    {
        public static string Create(XDocument source)
        {
            return $@"<!DOCTYPE html><html><head>
<style>
{HtmlTemplate.Css}
{HtmlTemplate.Palette}
</style></head>
<body>
<div class=""root"">
{Body(source)}
</div>
<script>
{HtmlTemplate.Js}
</script>
</body>
</html>
";
        }
        
        private static string Body(XDocument source)
        {
            var root = source.Element("watch");
            if (root == null) throw new InvalidOperationException("No 'watch' root element.");

            var totalTicks = long.TryParse(root.Attribute("totalTicks")?.Value ?? "", out var tt) ? tt : 0;
            var rootSplit = new Split
            {
                Start = TimeSpan.Zero,
                End = TimeSpan.FromTicks(totalTicks)
            };

            var splits = root.Elements("split")
                .Select(e => Split.FromXml(e,  rootSplit))
                .OrderBy(b => b.Start)
                .ToArray();
            
            var x = TimeSpan.Zero;
            splits = Elide(splits, ref x).ToArray();
             rootSplit.End =  rootSplit.End.Subtract(x);
            
            return string.Join(Environment.NewLine, splits.Select((b,i) => SplitHtml.Create(b, false, i)));
        }

        private static IList<Split> Elide(IList<Split> source, ref TimeSpan shorten)
        {
            var output = new List<Split>();
            if (source.Count == 0) return output;
            foreach (var split in source)
            {
                if (split.Elide)
                {
                    if (split.Parent != null)
                    {
                        split.Parent.Elapsed -= split.Elapsed;
                        split.Parent.End = split.Parent.End.Subtract(TimeSpan.FromMilliseconds(split.Elapsed));
                    }
                    shorten = shorten.Add(TimeSpan.FromMilliseconds(split.Elapsed));
                    continue;
                }

                split.Start = split.Start.Subtract(shorten);
                split.End = split.End.Subtract(shorten);
                if (split.Subs.Length > 0)
                {
                    split.Subs = Elide(split.Subs, ref shorten).ToArray();
                }
                output.Add(split);
            }

            return output;
        }
    }
}