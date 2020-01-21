using System;

namespace HORNS_Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press 1 to run Example 1: \"Woodcutters and rzodkiews\".");
            Console.WriteLine("Press 2 to run Example 2: \"Feeling soupy\".");
            Console.WriteLine("Press 3 to run Example 3: \"Chair collectors\".");

            bool valid = false;
            while (!valid)
            {
                var input = Console.ReadLine();
                switch(input)
                {
                    case "1":
                        valid = true;
                        Example1.Run();
                        break;
                    case "2":
                        valid = true;
                        Example2.Run();
                        break;
                    case "3":
                        valid = true;
                        Example3.Run();
                        break;
                }
                if (!valid)
                {
                    Console.WriteLine("Wrong answer, try again.");
                }
            }
        }
    }
}
