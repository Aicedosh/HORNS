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

        public IntegerPrecondition(IntegerPrecondition precondition) : base(precondition)
        {
            solver = precondition.solver;
            Direction = precondition.Direction;
        }

        protected internal override Precondition Combine(Precondition precondition)
        {
            if (!(precondition is IntegerPrecondition intPre) || Direction != intPre.Direction)
            {
                return null;
            }
            return new IntegerPrecondition(Variable, Value + intPre.Value, Direction, solver);
        }

        protected internal override bool IsEqualOrWorse(Precondition precondition)
        {
            if (!(precondition is IntegerPrecondition intPre) || Direction != intPre.Direction || Value != intPre.Value)
            {
                return false;
            }
            return Variable.Value >= intPre.Variable.Value;
        }

        protected internal override Precondition Subtract(ActionResult actionResult)
        {
            var addRes = actionResult as IntegerAddResult;
            int newVal = Value + addRes.Term * (Direction == Condition.AtMost ? 1 : -1);

            return new IntegerPrecondition(Variable, newVal, Direction, solver);
        }

        protected internal override bool IsFulfilled(int value)
        {
            return Direction == Condition.AtLeast ? value >= Value : value <= Value;
        }

        protected internal override bool IsZeroed(int value)
        {
            return value <= 0;
        }

        protected internal override Precondition Clone()
        {
            return new IntegerPrecondition(this);
        }
    }
}
