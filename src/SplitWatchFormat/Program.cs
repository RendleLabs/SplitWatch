using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RendleLabs.Diagnostics.SplitWatchFormat
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var xmlFile = CheckFileArg(args);

            XDocument doc;
            using (var reader = File.OpenText(xmlFile))
            {
                doc = XDocument.Load(reader);
            }

            var target = Path.GetFileNameWithoutExtension(xmlFile) + ".html";
            using (var writer = File.CreateText(target))
            {
                writer.WriteLine(HtmlReport.Create(doc));
            }
        }

        private static string CheckFileArg(string[] args)
        {
            var xmlFile = args.LastOrDefault();
            if (string.IsNullOrWhiteSpace(xmlFile))
            {
                Console.Error.WriteLine("Usage: ttformat sourceXmlFile");
                Environment.Exit(1);
            }

            if (!File.Exists(xmlFile))
            {
                Console.Error.WriteLine($"File not found: {xmlFile}");
                Environment.Exit(2);
            }
            
            return xmlFile;
        }
    }
}
