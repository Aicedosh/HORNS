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
        class Solver1 : HORNS.VariableSolver<bool, Result1>
        {
            private List<Result1> results = new List<Result1>();

            protected override void Register(Result1 result)
            {
                throw new NotImplementedException();
            }

            protected override IEnumerable<HORNS.Action> GetActions(Variable<bool> variable, bool goalValue)
            {
                return results.Where(r => r.V == goalValue).Select(r => r.Action);
            }
        }

        class Result1 : HORNS.ActionResult<bool, Solver1>
        {
            public bool V = true;

            public Result1(Variable<bool> var) : base(var)
            { }

            public override void Apply()
            {
                Variable.Value = V;
            }

            public override double GetCost() //TODO: Make abstract "GoalValue" that is computed during "simulation" and get cost based on that in parent class
            {
                return Variable.Evaluate(V) - Variable.Evaluate(Variable.Value);
            }
        }

        class Need1 : Need<bool>
        {
            public Need1(Variable<bool> variable, bool desired) : base(variable, desired)
            {
            }

            public override float Evaluate(bool value)
            {
                return value ? 100 : 0;
            }
        }

        class MyAction : HORNS.Action
        {
            protected override void ActionResult()
            {
                Console.WriteLine("Hello");
            }
        }


        static void Main(string[] args)
        {
            Agent agent = new Agent();
            Solver1 solver = new Solver1();

            HORNS.Action a = new MyAction();
            Variable<bool> dummy = new Variable<bool>();
            a.AddResult<bool, Result1, Solver1>(new Result1(dummy), solver);

            Need1 n = new Need1(dummy, false);

            agent.AddAction(a);
            agent.AddNeed(n);
        }
    }
}
