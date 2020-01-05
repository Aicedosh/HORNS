using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public enum IntegerDirection { AtLeast, AtMost }
    /// <summary>
    /// DO NOT USE YET
    /// </summary>
    public class IntegerSimplePrecondition : Precondition<int>
    {
        public IntegerDirection Direction { get; }
        /// <summary>
        /// DO NOT USE YET
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <param name="worst"></param>
        public IntegerSimplePrecondition(int value, IntegerDirection comparison, int worst = 0) : base(value)
        {
            Direction = comparison;
        }

        public IntegerSimplePrecondition(IntegerSimplePrecondition precondition) : base(precondition)
        {
            Direction = precondition.Direction;
        }

        private IntegerSimplePrecondition(int value, int state, IntegerSimplePrecondition other) : base(value, state, other)
        {
        }

        protected internal override bool IsFulfilled(int value, int target)
        {
            return Direction == IntegerDirection.AtLeast ? value >= target : value <= target;
        }

        protected internal override bool IsFulfilledInState(int value, int target, int state)
        {
            return Direction == IntegerDirection.AtLeast ? value + state >= target : value - state <= target;
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

        protected internal override ComparisonResult IsBetterThan(Precondition precondition)
        {
            if (!(precondition is IntegerSimplePrecondition intPre)
                || Variable.Id != intPre.Variable.Id
                || Direction != intPre.Direction
                || Target != intPre.Target)
            {
                return ComparisonResult.NotComparable;
            }

            if (Direction == IntegerDirection.AtLeast)
            {
                return State > intPre.State ? ComparisonResult.Better : ComparisonResult.EqualWorse;
            }
            else
            {
                return State < intPre.State ? ComparisonResult.Better : ComparisonResult.EqualWorse;
            }
        }

        protected internal override Precondition Combine(Precondition precondition)
        {
            if (!(precondition is IntegerSimplePrecondition intPre)
                || Variable.Id != intPre.Variable.Id
                || Direction != intPre.Direction)
            {
                return null;
            }
            
            int target = Direction == IntegerDirection.AtLeast ? Math.Max(Target, intPre.Target) : Math.Min(Target, intPre.Target);
            int state = State + intPre.State - GetDefault();
            return new IntegerSimplePrecondition(target, state, this);
        }
    }
}
