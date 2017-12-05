using System;
using System.Collections.Generic;

namespace ActivityReport
{
    public class Entry
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Source { get; set; }
        public string Key { get; set; }
        public string Operation { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public Entry Parent { get; set; }
        public List<Entry> Children { get; } = new List<Entry>();
    }
}