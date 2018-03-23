using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace TimeMachineTests
{
    public class WorkQueue
    {
        public List<string> Items { get; set; } = new List<string>();
        public void Add(string item)
        {
            Items.Add(item);
            Thread.Sleep(item.Length * 1000);
            Debug.WriteLine($"Completed {item}");
        }
    }
}
