using System;
using System.IO;

namespace HORNS_Performance
{
    static class Logger
    {
        public static bool WriteToFile { get; set; } = true;
        static StreamWriter writer;
        
        public static void Log(string message)
        {
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
    }
}
