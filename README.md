# SplitWatch
A hierarchical alternative to Stopwatch

This is something I threw together to record timings for my C# code in a tree structure, so you can record how long
various sub-tasks and sub-sub-tasks and so on take. It's more of a debugging tool than a production metrics system.

## Example usage
```c#
public void SomeMethod()
{
    var watch = SplitWatch.StartNew("Overall thing");
    using (watch.Split("First job"))
    {
        DoFirstJob();
    }
    string input;
    // Use ignore to elide timings from the end results,
    // e.g. we don't want to time waiting for user input, do we?
    using (watch.Ignore("Wait for user input"))
    {
        input = Console.ReadLine();
    }
    using (var sub = watch.Split("Complex second job"))
    {
        DoSecondJob(input, sub);
    }
    watch.Stop();
    watch.SaveXml("splitwatch.xml");
}
```

The XML that is output gets elements backwards because of the way the tree structure is stored internally. But you can
use the Format tool to turn it into an HTML file with embedded styling and functionality, so you can share it easily or
check it into source control or whatever. Thanks to the magic of .NET [CoreRT](https://github.com/dotnet/corert) there's
actually a native `swformat.exe` executable - no runtime required - which you can download from the [Releases](https://github.com/RendleLabs/SplitWatch/releases) here.
At some point I'll get round to doing a Linux native as well.
