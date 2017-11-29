using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Linq;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo("SplitWatch.UnitTests")]

namespace RendleLabs.Diagnostics
{
    [PublicAPI]
    public sealed class SplitTimer : IDisposable
    {
        private readonly string _tag;
        private readonly bool _elide;
        private readonly string _callerFilePath;
        private readonly string _callerMemberName;
        private readonly int _callerLineNumber;
        private readonly int _threadId = Thread.CurrentThread.ManagedThreadId;
        private readonly bool _poolThread = Thread.CurrentThread.IsThreadPoolThread;
        private readonly long _startTicks;
        private long _endTicks;
        private SplitTimer _child;
        private SplitTimer _sibling;

        internal SplitTimer(string tag, bool elide, string callerFilePath, string callerMemberName, int callerLineNumber)
        {
            _tag = tag;
            _elide = elide;
            _callerFilePath = callerFilePath;
            _callerMemberName = callerMemberName;
            _callerLineNumber = callerLineNumber;
            _startTicks = _endTicks = DateTime.UtcNow.Ticks;
        }

        public SplitTimer Split([NotNull] string tag, [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            return Create(tag, false, callerFilePath, callerMemberName, callerLineNumber);
        }

        public SplitTimer Ignore([NotNull] string tag, [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            return Create(tag, true, callerFilePath, callerMemberName, callerLineNumber);
        }

        private SplitTimer Create(string tag, bool elide, string file, string member, int line)
        {
            var newTimer = new SplitTimer(tag, elide, file, member, line);
            var oldTimer = Interlocked.Exchange(ref _child, newTimer);
            _child._sibling = oldTimer;
            return newTimer;
            
        }

        public XElement ToXml(long baseTicks)
        {
            var x = new XElement("split",
                new XAttribute("tag", _tag),
                new XAttribute("file", _callerFilePath),
                new XAttribute("member", _callerMemberName),
                new XAttribute("line", _callerLineNumber),
                new XAttribute("thread", _threadId),
                new XAttribute("poolThread", _poolThread),
                new XAttribute("elide", _elide),
                new XAttribute("start", TimeSpan.FromTicks(_startTicks - baseTicks)),
                new XAttribute("end", TimeSpan.FromTicks(_endTicks - baseTicks)),
                new XAttribute("elapsed", TimeSpan.FromTicks(_endTicks - _startTicks).TotalMilliseconds));

            if (_child != null)
            {
                x.Add(Children().Select(c => c.ToXml(baseTicks)));
            }

            return x;
        }

        internal void Stop()
        {
            _endTicks = DateTimeOffset.UtcNow.Ticks;
        }

        public void Dispose()
        {
            Stop();
        }

        internal IEnumerable<SplitTimer> Children()
        {
            var child = _child;
            while (child != null)
            {
                yield return child;
                child = child._sibling;
            }
        }
    }
}