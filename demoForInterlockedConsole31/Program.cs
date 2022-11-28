using System;
using System.Threading;

namespace demoForInterlockedConsole31
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var age = 25;
            Console.WriteLine($"origin age:{age}");
            var data = Interlocked.Exchange(ref age, 0);
            Console.WriteLine($"data result:{data}");
            Console.WriteLine($"new age:{age}");

            Console.ReadKey();
        }
    }
}
