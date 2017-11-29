using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using RendleLabs.Diagnostics;
using Xunit;

namespace SplitWatch.UnitTests
{
    public class SplitTests
    {
        [Fact]
        public void DoesChildren()
        {
            var a = new SplitTimer("a", false, "", "", 0);
            var b = a.Split("b");
            var c = a.Split("c");
            b.Dispose();
            c.Dispose();
            
            Assert.Equal(2, a.Children().Count());
        }
    }
}
