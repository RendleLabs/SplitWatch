using System;
using System.Threading;

namespace SplitWatch.TestApp
{
    class Program
    {
        static readonly Random Random = new Random(42);
        static void Main(string[] args)
        {
            var timer = RendleLabs.Diagnostics.SplitWatch.StartNew("Entry");
            using (timer.Split("Task 1"))
            {
                Sleep();
            }
            using (var sub = timer.Split("Task 2"))
            {
                using (sub.Split("Sub-task 2.1"))
                {
                    Sleep();
                }
                using (sub.Split("Sub-task 2.2"))
                {
                    Sleep();
                }
                using (sub.Split("Sub-task 2.3"))
                {
                    Sleep();
                }
            }
            using (timer.Ignore("Task 3"))
            {
                Sleep();
            }
            using (timer.Split("Task 4"))
            {
                Sleep();
            }
            timer.Stop();

            timer.Save("time.xml");
        }

        static void Sleep()
        {
            Thread.Sleep(Random.Next(100, 1000));
        }
    }
}