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

            // just a reminder that we really shouldn't be modifying variables in ActionResult()...
            // it'll break pathfinding since we can't apply this result to requirements
            protected override void ActionResult()
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
            }
        }

        private class PickAction : HORNS.Action
        {
            protected override void ActionResult()
            {
                Console.WriteLine("Pick axe");
            }
        }

        private class SleepAction : HORNS.Action
        {
            protected override void ActionResult()
            {
                Console.WriteLine("Sleep");
            }
        }

        private class PutAction : HORNS.Action
        {
            protected override void ActionResult()
            {
                Console.WriteLine("Put axe");
            }
        }

        private class BoredAction : HORNS.Action
        {
            protected override void ActionResult()
            {
                Console.WriteLine("Be bored");
            }
        }

        private class Woodesire : Need<bool>
        {
            public Woodesire(Variable<bool> variable, bool desired, VariableSolver<bool> solver) : base(variable, desired, solver)
            {
            }

            public override float Evaluate(bool value)
            {
                return value ? 100 : 0;
            }
        }

        private class SleepNeed : Need<bool>
        {
            public SleepNeed(Variable<bool> variable, bool desired, VariableSolver<bool> solver) : base(variable, desired, solver)
            {
            }

            public override float Evaluate(bool value)
            {
                return value ? 10000 : 1;
            }
        }

        private class GetRzodkiewAction : HORNS.Action
        {
            protected override void ActionResult()
            {
                Console.WriteLine("Spawn rzodkiew");
            }
        }

        private class RzodkiewNeed : Need<int>
        {
            public RzodkiewNeed(Variable<int> variable, int desired, VariableSolver<int> solver) : base(variable, desired, solver)
            {
            }

            public override float Evaluate(int value)
            {
                return value - 1;
            }
        }


        public static void Run()
        {
            Agent agent = new Agent();
            BooleanSolver treeSolver = new BooleanSolver();
            BooleanSolver axeSolver = new BooleanSolver();
            BooleanSolver energySolver = new BooleanSolver();
            IntegerSolver rzodkiewSolver = new IntegerSolver();

            Variable<bool> hasTree = new Variable<bool>() { Value = false };
            Variable<bool> hasAxe = new Variable<bool>() { Value = false };
            Variable<bool> hasEnergy = new Variable<bool>() { Value = true };
            Variable<int> rzodkiews = new Variable<int>() { Value = 5 };

            HORNS.Action chop = new ChopAction(hasEnergy, rzodkiews, 3);
            chop.AddCost(5);
            chop.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(hasAxe, true, axeSolver));
            chop.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(hasEnergy, true, energySolver));
            chop.AddPrecondition<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(new IntegerPrecondition(rzodkiews, 1, IntegerPrecondition.Condition.AtLeast, rzodkiewSolver));
            chop.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(hasTree, true), treeSolver);

            HORNS.Action chop2 = new ChopAction(hasEnergy, rzodkiews, 3, "without axe");
            chop2.AddCost(2);
            chop2.AddCost(rzodkiews, rz => rz);
            chop2.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(hasEnergy, true, energySolver));
            chop2.AddPrecondition<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(new IntegerPrecondition(rzodkiews, 1, IntegerPrecondition.Condition.AtLeast, rzodkiewSolver));
            chop2.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(hasTree, true), treeSolver);

            HORNS.Action pick = new PickAction();
            pick.AddCost(1);
            pick.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(hasAxe, false, axeSolver));
            pick.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(hasAxe, true), axeSolver);

            HORNS.Action put = new PutAction();
            put.AddCost(1);
            put.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(hasAxe, true, axeSolver));
            put.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(hasAxe, false), axeSolver);

            HORNS.Action sleep = new SleepAction();
            sleep.AddCost(100);
            sleep.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(hasAxe, false, axeSolver));
            sleep.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(hasEnergy, true), energySolver);

            Woodesire n = new Woodesire(hasTree, true, treeSolver);
            SleepNeed sleepNeed = new SleepNeed(hasEnergy, true, energySolver);
            
            agent.AddActions(chop, chop2, pick, put, sleep);

            agent.AddNeed(n);
            agent.AddNeed(sleepNeed);

            HORNS.Action getRzodkiew = new GetRzodkiewAction();
            getRzodkiew.AddCost(3);
            getRzodkiew.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(rzodkiews, 1), rzodkiewSolver);
            RzodkiewNeed rzodkiewNeed = new RzodkiewNeed(rzodkiews, 1, rzodkiewSolver);

            agent.AddAction(getRzodkiew);
            //agent.AddNeed(rzodkiewNeed);

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
