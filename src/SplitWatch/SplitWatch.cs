using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace RendleLabs.Diagnostics
{
    [PublicAPI]
    public sealed class SplitWatch
    {
        private readonly DateTimeOffset _startTime;
        private DateTimeOffset _endTime;
        private readonly long _baseTickCount;
        private readonly SplitTimer _root;

        private SplitWatch(string tag, string callerFilePath, string callerMemberName, int callerLineNumber)
        {
            _baseTickCount = (_startTime = DateTimeOffset.UtcNow).Ticks;
            _root = new SplitTimer(tag, false, callerFilePath, callerMemberName, callerLineNumber);
        }

        [PublicAPI]
        public static SplitWatch StartNew([NotNull] string tag, [CallerFilePath]string callerFilePath = null, [CallerMemberName]string callerMemberName = null, [CallerLineNumber]int callerLineNumber = 0)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            return new SplitWatch(tag, callerFilePath, callerMemberName, callerLineNumber);
        }

        [PublicAPI]
        public SplitTimer Split([NotNull] string tag, [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable ExplicitCallerInfoArgument
            return _root.Split(tag, callerFilePath, callerMemberName, callerLineNumber);
            // ReSharper restore ExplicitCallerInfoArgument
        }

        [PublicAPI]
        public SplitTimer Ignore([NotNull] string tag, [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
        {
            // ReSharper disable ExplicitCallerInfoArgument
            return _root.Ignore(tag, callerFilePath, callerMemberName, callerLineNumber);
            // ReSharper restore ExplicitCallerInfoArgument
        }
        
        [PublicAPI]
        public void Stop()
        {
            _root.Stop();
            _endTime = DateTimeOffset.UtcNow;
        }

        [PublicAPI]
        public XElement ToXml()
        {
            var totalTicks = _endTime.Ticks - _baseTickCount;
            var root = new XElement("watch",
                new XAttribute("startTime", _startTime),
                new XAttribute("totalTicks", totalTicks),
                _root.ToXml(_baseTickCount));
            return root;
        }

        [PublicAPI]
        public XDocument Save([NotNull] string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            var doc = new XDocument(ToXml());
            
            using (var writer = File.CreateText(path))
            {
                doc.Save(writer);
            }

            return doc;
        }
    }
}
