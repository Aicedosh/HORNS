using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class IntegerPrecondition : Precondition<int, IntegerSolver>
    {
        private readonly IntegerSolver solver;

        public enum Condition
        {
            AtLeast, AtMost
        }
        public Condition Direction { get; }

        public IntegerPrecondition(Variable<int> variable, int value, Condition direction, IntegerSolver solver)
            : base(variable, value, solver)
        {
            Direction = direction;
            this.solver = solver;
        }

        protected override Precondition<int> Combine(Precondition<int> pre)
        {
            if (!(pre is IntegerPrecondition intPre) || Direction != intPre.Direction)
            {
                return null;
            }
            return new IntegerPrecondition(Variable, Value + intPre.Value, Direction, solver);
        }

        protected override bool IsEqualOrWorse(Precondition<int> pre)
        {
            if (!(pre is IntegerPrecondition intPre) || Direction != intPre.Direction || Value != intPre.Value)
            {
                return false;
            }
            return Variable.Value >= intPre.Variable.Value;
        }

        internal override PreconditionRequirement Subtract(PreconditionRequirement req, ActionResult<int> result)
        {
            var addRes = result as IntegerAddResult;
            bool plus = true;
            if (addRes.Term < 0)
            {
                plus = Direction == Condition.AtLeast;
            }
            else
            {
                plus = Direction == Condition.AtMost;
            }

            int newVal = Value + addRes.Term * (plus ? 1 : -1);

            return new IntegerPrecondition(Variable, newVal, Direction, solver).GetRequirement() as PreconditionRequirement;
        }

        protected internal override bool IsFulfilled(int value)
        {
            return Direction == Condition.AtLeast ? value >= Value : value <= Value;
        }

        protected internal override bool IsReqZeroed(int value)
        {
            return value <= 0;
        }
    }
}
