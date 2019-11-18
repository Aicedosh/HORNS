using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HORNS;

namespace HORNS_Sandbox
{
    class Program
    {
        class ChopAction : HORNS.Action
        {
            private readonly Variable<bool> energy;

            public ChopAction(Variable<bool> energy)
            {
                this.energy = energy;
            }

            protected override void ActionResult()
            {
                Console.WriteLine("Chop");
                Random random = new Random();
                if(random.Next(5) == 0 && false)
                {
                    energy.Value = false;
                    Console.WriteLine(" Tired");
                }
            }
        }

        class PickAction : HORNS.Action
        {
            protected override void ActionResult()
            {
                Console.WriteLine("Axe pickup");
            }
        }

        class Woodesire : Need<bool>
        {
            public Woodesire(Variable<bool> variable, bool desired, VariableSolver<bool> solver) : base(variable, desired, solver)
            {
            }

            public override float Evaluate(bool value)
            {
                return value ? 100 : 0;
            }
        }

        class SleepNeed : Need<bool>
        {
            public SleepNeed(Variable<bool> variable, bool desired, VariableSolver<bool> solver) : base(variable, desired, solver)
            {
            }

            public override float Evaluate(bool value)
            {
                return value ? 100 : 0;
            }
        }


        static void Main(string[] args)
        {
            Agent agent = new Agent();
            BooleanSolver treeSolver = new BooleanSolver();
            BooleanSolver axeSolver = new BooleanSolver();
            BooleanSolver energySolver = new BooleanSolver();

            Variable<bool> hasTree = new Variable<bool>() { Value = false };
            Variable<bool> hasAxe = new Variable<bool>() { Value = false };
            Variable<bool> hasEnergy = new Variable<bool>() { Value = true };

            HORNS.Action chop = new ChopAction(hasEnergy);
            chop.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(hasAxe, true, axeSolver));
            chop.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(hasEnergy, true, energySolver));
            chop.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(hasTree, true), treeSolver);

            HORNS.Action pick = new PickAction();
            pick.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(hasAxe, false, axeSolver));
            pick.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(hasAxe, true), axeSolver);

            HORNS.Action put = new PutAction();
            put.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(hasAxe, true, axeSolver));
            put.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(hasAxe, false), axeSolver);

            HORNS.Action sleep = new SleepAction();
            sleep.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(hasAxe, false, axeSolver));
            sleep.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(hasEnergy, true), energySolver);

            Woodesire n = new Woodesire(hasTree, true, treeSolver);
            SleepNeed sleepNeed = new SleepNeed(hasEnergy, true, energySolver);

            agent.AddAction(chop);
            agent.AddAction(pick);
            agent.AddAction(put);
            agent.AddAction(sleep);

            agent.AddNeed(n);
            agent.AddNeed(sleepNeed);

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

    internal class SleepAction : HORNS.Action
    {
        protected override void ActionResult()
        {
            Console.WriteLine("Sleep");
        }
    }

    internal class PutAction : HORNS.Action
    {
        protected override void ActionResult()
        {
            Console.WriteLine("Put axe");
        }
    }
}
