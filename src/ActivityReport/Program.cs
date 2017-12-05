using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReport
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            var directory = args.FirstOrDefault() ?? Environment.CurrentDirectory;
            var entries = await Entries(directory);
            var report = new HtmlReport(entries);
            using (var writer = File.CreateText("activityreport.html"))
            {
                await report.Write(writer);
            }
        }

        private static async Task<Entry[]> Entries(string directory)
        {
            var files = new Files().Find(directory).ToArray();
            var entryParser = new EntryParser();
            var contents = new FileReader().ReadAll(files);
            var entries = contents.SelectAsync(c => entryParser.ParseXml(c));
            var dict = (await Task.WhenAll(entries)).ToDictionary(e => e.Id);
            foreach (var entry in dict.Values.Where(e => !string.IsNullOrWhiteSpace(e.ParentId)))
            {
                if (!dict.TryGetValue(entry.ParentId, out var parent)) continue;
                
                entry.Parent = parent;
                parent.Children.Add(entry);
            }

            return dict.Values.Where(e => e.Parent == null).ToArray();
        }
        
    }

    public class FileReader
    {
        public IEnumerable<Task<string>> ReadAll(IEnumerable<string> paths)
        {
            return paths.Select(Read);
        }

        private static async Task<string> Read(string path)
        {
            using (var reader = File.OpenText(path))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }

    public static class EnumerableAsync
    {
        public static IEnumerable<Task<TResult>> SelectAsync<T, TResult>(this IEnumerable<Task<T>> source, Func<T, TResult> map)
        {
            return source.Select(t => MapAsync(t, map));
        }

        private static async Task<TResult> MapAsync<T, TResult>(Task<T> source, Func<T, TResult> map)
        {
            return map(await source);
        }
    }
}