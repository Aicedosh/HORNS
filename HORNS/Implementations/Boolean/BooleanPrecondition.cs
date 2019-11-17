using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class BooleanPrecondition : Precondition<bool, BooleanSolver>
    {
        public BooleanPrecondition(Variable<bool> variable, bool value, BooleanSolver solver) : base(variable, solver)
        {
            Value = value;
        }

        public bool Value { get; }

        protected internal override bool IsFulfilled(bool value)
        {
            return value == Value;
        }
    }
}
