using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HORNS;

namespace HORNS_Sandbox
{
    class Example1
    {
        private class ChopAction : HORNS.Action
        {
            private readonly Variable<bool> energy;
            private readonly Variable<int> rzodkiews;
            private readonly int chanceDenominator;
            private readonly string message;

            // chance will be 1/chanceDenominator
            public ChopAction(Variable<bool> energy, Variable<int> rzodkiews, int chanceDenominator, string message = "")
            {
                this.energy = energy;
                this.rzodkiews = rzodkiews;
                this.chanceDenominator = chanceDenominator;
                this.message = message;
            }

            public override void Perform()
            {
                Console.WriteLine($"Chop {message}");
                Random random = new Random();
                if(random.Next(chanceDenominator) == 0)
                {
                    energy.Value = false;
                    Console.WriteLine(" Tired");
                }
                if (random.Next(chanceDenominator) == 0)
                {
                    rzodkiews.Value -= 1;
                    Console.WriteLine(" Ate a rzodkiew");
                }
                Apply();
            }
        }

        private class PickAction : HORNS.Action
        {
            public override void Perform()
            {
                Console.WriteLine("Pick axe");
                Apply();
            }
        }

        private class SleepAction : HORNS.Action
        {
            public override void Perform()
            {
                Console.WriteLine("Sleep");
                Apply();
            }
        }

        private class PutAction : HORNS.Action
        {
            public override void Perform()
            {
                Console.WriteLine("Put axe");
                Apply();
            }
        }

        private class BoredAction : HORNS.Action
        {
            public override void Perform()
            {
                Console.WriteLine("Be bored");
                Apply();
            }
        }

        private class Woodesire : Need<int>
        {
            public Woodesire(Variable<int> variable, int desired) : base(variable, desired, v => v, v => v >= desired)
            {
            }
        }

        private class SleepNeed : Need<bool>
        {
            public SleepNeed(Variable<bool> variable, bool desired) : base(variable, desired, v=>v ? 10000 : 1)
            {
            }
        }

        private class GetRzodkiewAction : HORNS.Action
        {
            public override void Perform()
            {
                Console.WriteLine("Spawn rzodkiew");
                Apply();
            }
        }

        private class RzodkiewNeed : Need<int>
        {
            public RzodkiewNeed(Variable<int> variable, int desired) : base(variable, desired, v => v, v => v >= desired)
            {
            }
        }


        public static void Run()
        {
            Agent agent = new Agent();

            IntegerVariable wood = new IntegerVariable(0);
            BooleanVariable hasAxe = new BooleanVariable(false);
            BooleanVariable hasEnergy = new BooleanVariable(true);
            IntegerVariable rzodkiews = new IntegerVariable(5);

            HORNS.Action chop = new ChopAction(hasEnergy, rzodkiews, 3);
            chop.AddCost(5);
            chop.AddPrecondition(hasAxe, new BooleanPrecondition(true));
            chop.AddPrecondition(hasEnergy, new BooleanPrecondition(true));
            chop.AddPrecondition(rzodkiews, new IntegerPrecondition(1, false));
            chop.AddResult(wood, new IntegerAddResult(1));

            HORNS.Action chop2 = new ChopAction(hasEnergy, rzodkiews, 3, "without axe");
            chop2.AddCost(2);
            chop2.AddCost(rzodkiews, rz => rz);
            chop2.AddPrecondition(hasEnergy, new BooleanPrecondition(true));
            chop2.AddPrecondition(rzodkiews, new IntegerPrecondition(1, false));
            chop2.AddResult(wood, new IntegerAddResult(1));

            HORNS.Action pick = new PickAction();
            pick.AddCost(1);
            pick.AddPrecondition(hasAxe, new BooleanPrecondition(false));
            pick.AddResult(hasAxe, new BooleanResult(true));

            HORNS.Action put = new PutAction();
            put.AddCost(1);
            put.AddPrecondition(hasAxe, new BooleanPrecondition(true));
            put.AddResult(hasAxe, new BooleanResult(false));

            HORNS.Action sleep = new SleepAction();
            sleep.AddCost(100);
            sleep.AddPrecondition(hasAxe, new BooleanPrecondition(false));
            sleep.AddResult(hasEnergy, new BooleanResult(true));

            Woodesire n = new Woodesire(wood, 100);
            SleepNeed sleepNeed = new SleepNeed(hasEnergy, true);
            
            agent.AddActions(chop, chop2, pick, put, sleep);

            agent.AddNeed(n);
            agent.AddNeed(sleepNeed);

            HORNS.Action getRzodkiew = new GetRzodkiewAction();
            getRzodkiew.AddCost(5);
            getRzodkiew.AddCost(wood, w => w);
            getRzodkiew.AddResult(rzodkiews, new IntegerAddResult(1));
            RzodkiewNeed rzodkiewNeed = new RzodkiewNeed(rzodkiews, 10);

            agent.AddAction(getRzodkiew);
            agent.AddNeed(rzodkiewNeed);

            for(; ; )
            {
                var nextAction = agent.GetNextAction();
                if (nextAction == null)
                {
                    Console.WriteLine("No plan was found!");
                }
                else
                {
                    nextAction.Perform();
                }
                Thread.Sleep(1000);
            }
        }
    }
}
