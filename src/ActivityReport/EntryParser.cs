using System;
using System.Globalization;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace ActivityReport
{
    public class EntryParser
    {
        [PublicAPI]
        public Entry ParseXml(string xmlString) => ParseXml(XElement.Parse(xmlString));

        [PublicAPI]
        public Entry ParseXml(XElement xml) => new Entry
        {
            Id = xml.Attribute("id")?.Value,
            ParentId = xml.Attribute("parentId")?.Value,
            Source = xml.Attribute("source")?.Value,
            Key = xml.Attribute("key")?.Value,
            Operation = xml.Attribute("operation")?.Value,
            Duration = ParseMilliseconds(xml.Attribute("duration")?.Value),
            StartTime = ParseDateTimeOffset(xml.Attribute("startTime")?.Value)
        };

        private static TimeSpan ParseMilliseconds(string milliseconds)
        {
            if (string.IsNullOrWhiteSpace(milliseconds)) return TimeSpan.Zero;
            return double.TryParse(milliseconds, out var ms) ? TimeSpan.FromMilliseconds(ms) : TimeSpan.Zero;
        }

        private static DateTimeOffset ParseDateTimeOffset(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString)) return default;
            return DateTimeOffset.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
                out var date)
                ? date
                : default;
        }
    }
}