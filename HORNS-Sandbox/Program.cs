using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HORNS;

namespace HORNS_Sandbox
{
    class Program
    {
        class Solver1 : HORNS.VariableSolver<bool, Result1, Precondition1>
        {
            private List<Result1> results = new List<Result1>();

            protected override void Register(Result1 result)
            {
                throw new NotImplementedException();
            }

            protected override IEnumerable<HORNS.Action> GetActionsSatisfying(Precondition1 precondition)
            {
                throw new NotImplementedException();
            }

            protected override IEnumerable<HORNS.Action> GetActionsTowards(Variable<bool> variable, bool desiredValue)
            {
                throw new NotImplementedException();
            }
        }

        class Result1 : HORNS.ActionResult<bool, Solver1>
        {
            public bool V = true;

            public Result1(Variable<bool> var) : base(var)
            { }

            protected override bool GetResultValue(Variable<bool> variable)
            {
                return V;
            }
        }

        class Precondition1 : HORNS.Precondition<bool, Solver1>
        {
            public Precondition1(Variable<bool> variable, Solver1 solver) : base(variable, solver)
            {
            }

            protected override bool IsFulfilled(bool value)
            {
                throw new NotImplementedException();
            }
        }

        class Need1 : Need<bool>
        {
            public Need1(Variable<bool> variable, bool desired, Solver1 solver) : base(variable, desired, solver)
            {
            }

            public override float Evaluate(bool value)
            {
                return value ? 100 : 0;
            }
        }

        class ChopAction : HORNS.Action
        {
            protected override void ActionResult()
            {
                Console.WriteLine("Chop");
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


        static void Main(string[] args)
        {
            Agent agent = new Agent();
            BooleanSolver solver = new BooleanSolver();

            Variable<bool> hasTree = new Variable<bool>() { Value = false };
            Variable<bool> hasAxe = new Variable<bool>() { Value = false };

            HORNS.Action chop = new ChopAction();
            chop.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(hasAxe, true, solver));
            chop.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(hasTree, true), solver);

            HORNS.Action pick = new PickAction();
            chop.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(hasAxe, false, solver));
            chop.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(hasAxe, false), solver);

            Woodesire n = new Woodesire(hasTree, true, solver);

            agent.AddAction(chop);
            agent.AddAction(pick);
            agent.AddNeed(n);
        }
    }
}
