using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class BooleanPrecondition : Precondition<bool, BooleanSolver>
    {
        private readonly BooleanSolver solver;

        public BooleanPrecondition(Variable<bool> variable, bool value, BooleanSolver solver) : base(variable, solver)
        {
            Value = value;
            this.solver = solver;
        }

        public bool Value { get; }

        protected override bool IsEqual(Precondition<bool> pre)
        {
            var boolPre = pre as BooleanPrecondition;
            if (pre == null)
            {
                return false;
            }
            return Value == boolPre.Value;
        }

        protected override PreconditionRequirement Subtract(PreconditionRequirement req, ActionResult<bool> result)
        {
            return new BooleanPrecondition(Variable, !(result as BooleanResult).EndValue, solver).GetRequirement() as PreconditionRequirement; //TODO: refactor
        }

        protected internal override bool IsFulfilled(bool value)
        {
            return value == Value;
        }
    }
}
