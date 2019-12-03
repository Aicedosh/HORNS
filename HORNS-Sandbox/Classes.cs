using System;
using System.Threading;
using HORNS;

namespace HORNS_Sandbox
{
    public class MessageAction : HORNS.Action
    {
        public string Message { get; }

        public MessageAction(string message)
        {
            Message = message;
        }

        public override void Perform()
        {
            Console.WriteLine(Message);
            Apply();
        }
    }

    public class TimeAction : HORNS.Action
    {
        public string Message { get; }
        int Times { get; }

        public TimeAction(string message, int times)
        {
            Message = message;
            Times = times;
        }

        public override void Perform()
        {
            Console.Write(Message);
            for (int i = 0; i < Times; i++)
            {
                Thread.Sleep(400);
                Console.Write(".");
            }
            Console.WriteLine();
        }
    }

    public class Hunger : Need<int>
    {
        public Hunger(Variable<int> variable, int desired) : base(variable, desired, v =>
        v > 100 ? -100 :
        (float)(50 * Math.Log10(-v + 1 + 100)))
        { }
    }

    public class Energy : Need<int>
    {
        public Energy(Variable<int> variable, int desired) : base(variable, desired, v =>
        v < 0 ? 100 : (float)(50 * Math.Log10(v + 1)))
        { }

        protected override bool IsSatisfied(int value)
        {
            return value >= 100;
        }
    }
}
