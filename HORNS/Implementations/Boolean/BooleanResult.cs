using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class BooleanResult : ActionResult<bool, BooleanSolver>
    {
        public BooleanResult(Variable<bool> variable, bool endValue) : base(variable)
        {
            EndValue = endValue;
        }

        public bool EndValue { get; }

        protected internal override bool GetResultValue(Variable<bool> variable)
        {
            return EndValue;
        }
    }
}
