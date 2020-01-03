using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public enum IntegerComparison { AtLeast, AtMost }

    public class IntegerSimplePrecondition : Precondition<int>
    {
        public IntegerComparison Comparison { get; }

        public IntegerSimplePrecondition(int value, IntegerComparison comparison, int worst = 0) : base(value)
        {
            Comparison = comparison;
        }

        public IntegerSimplePrecondition(IntegerSimplePrecondition precondition) : base(precondition)
        {
            Comparison = precondition.Comparison;
        }

        private IntegerSimplePrecondition(int value, int state, IntegerSimplePrecondition other) : base(value, state, other)
        {
        }

        protected internal override bool IsFulfilled(int value, int target)
        {
            return Comparison == IntegerComparison.AtLeast ? value >= target : value <= target;
        }

        protected internal override bool IsFulfilledInState(int value, int target, int state)
        {
            return Comparison == IntegerComparison.AtLeast ? value + state >= target : value - state <= target;
        }

        protected internal override int GetDefault()
        {
            // TODO: don't really like this... we could create some sort of IBounded or assume that WorstBound is zero
            if (Variable is IntegerSimpleVariable var)
            {
                return var.WorstBound;
            }
            return 0;
        }

        protected internal override Precondition Clone()
        {
            return new IntegerSimplePrecondition(this);
        }

        protected internal override bool IsBetterThan(Precondition precondition)
        {
            if (!(precondition is IntegerSimplePrecondition intPre)
                || Variable.Id != intPre.Variable.Id
                || Comparison != intPre.Comparison
                || Target != intPre.Target)
            {
                return false;
            }
            return Comparison == IntegerComparison.AtLeast ? State > intPre.State : State < intPre.State;
        }

        protected internal override Precondition Combine(Precondition precondition)
        {
            if (!(precondition is IntegerSimplePrecondition intPre)
                || Variable.Id != intPre.Variable.Id
                || Comparison != intPre.Comparison)
            {
                return null;
            }
            
            int target = Comparison == IntegerComparison.AtLeast ? Math.Max(Target, intPre.Target) : Math.Min(Target, intPre.Target);
            int state = State + intPre.State - GetDefault();
            return new IntegerSimplePrecondition(target, state, this);
        }
    }
}
