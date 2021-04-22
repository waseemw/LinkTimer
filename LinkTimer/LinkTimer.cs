using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Chronic.Core;
using Chronic.Core.System;

namespace LinkTimer
{
    internal class LinkTimer
    {
        private readonly List<Link> _linkList = new();

        public void Run(string fileName)
        {
            ReadFile(fileName);
            PrintAll();
        }

        private void ReadFile(string fileName)
        {
            var lines = File.ReadLines(fileName);
            foreach (var line in lines) AddLineToList(line);
        }

        private void AddLineToList(string line)
        {
            var items = line.Split(new[] {"::", "--", ";;"}, StringSplitOptions.TrimEntries);
            var times = new DateTime[items.Length - 2];
            var parser = new Parser();
            for (var i = 2; i < items.Length; i++) times[i - 2] = parser.Parse(items[i]).ToTime();
            _linkList.Add(new Link {Name = items[0], Url = items[1], Times = times});
        }
        
        private void PrintAll()
        {
            if (_linkList.Count > 0)
                Console.WriteLine("=========================");
            foreach (var link in _linkList)
            {
                Console.WriteLine($"Name: {link.Name}");
                Console.WriteLine($"Link: {link.Url}");
                link.Times.ForEach(d =>
                {
                    var weekSeconds = 604800;
                    var seconds = (d - DateTime.Now).TotalSeconds;
                    if (seconds < 0) seconds += weekSeconds;
                    else if (seconds > weekSeconds) seconds -= weekSeconds;
                    new Timer(_ =>
                        {
                            Console.WriteLine($"--{DateTime.Now.DayOfWeek} {DateTime.Now.ToShortTimeString()}: Starting {link.Name}");
                            new Process {StartInfo = {UseShellExecute = true, FileName = link.Url}}.Start();
                        }, null, (long) (seconds * 1000),
                        weekSeconds * 1000);
                    Console.WriteLine($"{d.DayOfWeek} {d.ToShortTimeString()} - Hour T-Minus: {seconds / 3600}");
                });
                Console.WriteLine("=========================");
            }
        }
    }


    internal class Link
    {
        public DateTime[] Times { get; init; }
        public string Name { get; init; }
        public string Url { get; init; }
    }
}