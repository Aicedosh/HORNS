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
            public override IEnumerable<HORNS.Action> GetActions(Variable<bool> variable, bool goalValue)
            {
                throw new NotImplementedException();
            }
        }

        class Result1 : HORNS.ActionResult<bool, Solver1>
        {
            public override void Apply()
            {
                throw new NotImplementedException();
            }

            public override double GetCost()
            {
                throw new NotImplementedException();
            }
        }


        static void Main(string[] args)
        {
            HORNS.Action a = new HORNS.Action();
            a.AddResult<bool, Result1, Solver1>(new Result1(), new Solver1());
        }
    }
}
