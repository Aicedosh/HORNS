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

        protected override Precondition<bool> Combine(Precondition<bool> pre)
        {
            if (!(pre is BooleanPrecondition boolPre) || Value != boolPre.Value)
            {
                return null;
            }
            return new BooleanPrecondition(Variable, Value, solver);
        }

        protected override bool IsEqualOrWorse(Precondition<bool> pre)
        {
            if (!(pre is BooleanPrecondition boolPre) || Value != boolPre.Value)
            {
                return false;
            }
            // pre will always be unfulfilled, which means that we're either equal or better
            return Variable.Value == boolPre.Variable.Value;
        }

        protected override PreconditionRequirement Subtract(PreconditionRequirement req, ActionResult<bool> result)
        {
            return new BooleanPrecondition(Variable, !(result as BooleanResult).EndValue, solver).GetRequirement() as PreconditionRequirement; //TODO: refactor
        }

        protected internal override bool IsFulfilled(bool value)
        {
            return value == Value;
        }

        // TODO: this is temporary
        protected internal override bool IsReqZeroed(bool value)
        {
            return value != Value;
        }
    }
}
