using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class BooleanPrecondition : Precondition<bool, BooleanSolver>
    {
        private readonly BooleanSolver solver;

        public BooleanPrecondition(Variable<bool> variable, bool value, BooleanSolver solver) : base(variable, value, solver)
        {
            this.solver = solver;
        }

        public BooleanPrecondition(BooleanPrecondition precondition) : base(precondition)
        {
            solver = precondition.solver;
        }

        protected internal override Precondition Combine(Precondition precondition)
        {
            if (!(precondition is BooleanPrecondition boolPre) || Value != boolPre.Value)
            {
                return null;
            }
            return new BooleanPrecondition(Variable, Value, solver);
        }

        protected internal override bool IsEqualOrWorse(Precondition precondition)
        {
            if (!(precondition is BooleanPrecondition boolPre) || Value != boolPre.Value)
            {
                return false;
            }
            // pre will always be unfulfilled, which means that we're either equal or better
            return Variable.Value == boolPre.Variable.Value;
        }

        protected internal override Precondition Subtract(ActionResult actionResult)
        {
            if (!(actionResult is BooleanResult))
            {
                return null;
            }
            return new BooleanPrecondition(Variable, !(actionResult as BooleanResult).EndValue, solver);
        }

        protected internal override bool IsFulfilled(bool value)
        {
            return value == Value;
        }

        // TODO: this is temporary
        protected internal override bool IsZeroed(bool value)
        {
            return value != Value;
        }

        protected internal override Precondition Clone()
        {
            return new BooleanPrecondition(this);
        }
    }
}
