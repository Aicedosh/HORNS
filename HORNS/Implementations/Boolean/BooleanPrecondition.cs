using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class BooleanPrecondition : Precondition<bool, BooleanSolver>
    {
        public BooleanPrecondition(bool value) : base(value)
        {
        }

        private BooleanPrecondition(bool value, BooleanPrecondition other) : base(value, other)
        {
        }

        public BooleanPrecondition(BooleanPrecondition precondition) : base(precondition)
        {
        }

        protected internal override Precondition Combine(Precondition precondition)
        {
            if (!(precondition is BooleanPrecondition boolPre) || Value != boolPre.Value)
            {
                return null;
            }
            return new BooleanPrecondition(Value, this);
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

        protected internal override Precondition Subtract(ActionResult result)
        {
            return new BooleanPrecondition(!(result as BooleanResult).EndValue, this);
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
