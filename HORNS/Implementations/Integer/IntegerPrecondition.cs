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
        public int Value { get; }

        public IntegerPrecondition(Variable<int> variable, int value, Condition direction, IntegerSolver solver)
            : base(variable, solver)
        {
            Value = value;
            Direction = direction;
            this.solver = solver;
        }

        protected override Precondition<int> Combine(Precondition<int> pre)
        {
            var intPre = pre as IntegerPrecondition;
            if (pre == null || Direction != intPre.Direction)
            {
                return null;
            }
            return new IntegerPrecondition(Variable, Value + intPre.Value, Direction, solver);
        }

        protected override bool IsEqual(Precondition<int> pre)
        {
            throw new NotImplementedException();
        }

        protected override PreconditionRequirement Subtract(PreconditionRequirement req, ActionResult<int> result)
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
