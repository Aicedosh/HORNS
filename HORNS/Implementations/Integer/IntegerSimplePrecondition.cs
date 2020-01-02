using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public enum IntegerComparison { AtLeast, AtMost }

    public class IntegerSimplePrecondition : Precondition<int>
    {
        public IntegerComparison Comparison { get; }
        // TODO: move to SimpleVariable
        public int WorstBound { get; }

        public IntegerSimplePrecondition(int value, IntegerComparison comparison, int worst = 0) : base(value)
        {
            Comparison = comparison;
            WorstBound = worst;
        }

        public IntegerSimplePrecondition(IntegerSimplePrecondition precondition) : base(precondition)
        {
            Comparison = precondition.Comparison;
            WorstBound = precondition.WorstBound;
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
            return WorstBound;
        }

        protected internal override Precondition Clone()
        {
            return new IntegerSimplePrecondition(this);
        }

        protected internal override bool IsBetterThan(Precondition precondition)
        {
            if (!(precondition is IntegerSimplePrecondition intPre)
                || Comparison != intPre.Comparison
                || Target != intPre.Target
                || WorstBound != intPre.WorstBound)
            {
                return false;
            }
            return Comparison == IntegerComparison.AtLeast ? State > intPre.State : State < intPre.State;
        }

        protected internal override Precondition Combine(Precondition precondition)
        {
            if (!(precondition is IntegerSimplePrecondition intPre)
                || Comparison != intPre.Comparison
                || WorstBound != intPre.WorstBound)
            {
                return null;
            }
            
            int target = Comparison == IntegerComparison.AtLeast ? Math.Max(Target, intPre.Target) : Math.Min(Target, intPre.Target);
            int state = State + intPre.State - WorstBound;
            return new IntegerSimplePrecondition(target, state, this);
        }
    }
}
