using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class IntegerAddResult : ActionResult<int, IntegerSolver>
    {
        public int Term { get; }

        public IntegerAddResult(int term)
        {
            Term = term;
        }

        protected internal override int GetResultValue(Variable<int> variable)
        {
            return variable.Value + Term;
        }
    }
}
