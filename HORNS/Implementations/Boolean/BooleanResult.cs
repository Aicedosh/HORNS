using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class BooleanResult : ActionResult<bool, BooleanSolver>
    {
        //TODO: [!] Replace with static factory methods?
        public BooleanResult(bool endValue)
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
