using System;
using System.IO;

namespace HORNS_Performance
{
    static class Logger
    {
        public static bool WriteToFile { get; set; } = true;
        static StreamWriter writer;
        
        public static void Log(int branches, int depth, int needCount, TimeSpan planTime)
        {
            string message = $"{branches}, {depth}, {needCount}, {planTime.TotalMilliseconds.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            if (WriteToFile)
            {
                if (writer == null)
                {
                    writer = new StreamWriter($"test-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.log");
                }
                writer.WriteLine(message);
            }
            Console.WriteLine(message);
        }

        public static void NewLog()
        {
            writer = null;
        }
    }
}
