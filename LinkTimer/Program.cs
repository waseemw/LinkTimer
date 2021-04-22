using System.Threading;

namespace LinkTimer
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            new LinkTimer().Run(args.Length > 0 ? args[0] : "Links.txt");
            new ManualResetEvent(false).WaitOne();
        }
    }
}