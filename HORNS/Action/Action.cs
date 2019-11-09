using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class Action
    {
        public void AddResult<T, RT, ST>(RT result, ST solver)
            where ST : VariableSolver<T, RT>
            where RT : ActionResult<T, ST>
        {
            solver.Register(result);
        }
    }
}
