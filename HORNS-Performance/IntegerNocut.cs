using System;
using System.Collections.Generic;
using System.Linq;
using HORNS;

namespace HORNS_Performance
{
    class IntegerNocutPrecondition : Precondition<int>
    {
        public IntegerNocutPrecondition(int target)
            : base(target)
        {
        }

        private IntegerNocutPrecondition(int target, int state, IntegerNocutPrecondition other) : base(target, state, other)
        {
        }

        public IntegerNocutPrecondition(IntegerNocutPrecondition precondition) : base(precondition)
        {
        }

        protected override Precondition Combine(Precondition precondition)
        {
            if (!(precondition is IntegerNocutPrecondition intPre))
            {
                return null;
            }
            int target = Math.Max(Target, intPre.Target);
            return new IntegerNocutPrecondition(target, State + intPre.State, this);
        }

        // NOCUT
        protected override ComparisonResult IsBetterThan(Precondition precondition)
        {
            return ComparisonResult.NotComparable;
        }

        protected override bool IsFulfilled(int value, int target)
        {
            return value >= target;
        }

        protected override bool IsFulfilledInState(int value, int target, int state)
        {
            return value + state >= target;
        }

        protected override int GetDefault()
        {
            return 0;
        }

        protected override Precondition Clone()
        {
            return new IntegerNocutPrecondition(this);
        }
    }
    
    class IntegerNocutSolver : VariableSolver<int, IntegerAddResult, IntegerNocutPrecondition>
    {
        List<IntegerAddResult> positiveResults = new List<IntegerAddResult>();
        List<IntegerAddResult> negativeResults = new List<IntegerAddResult>();
        
        protected override IEnumerable<HORNS.Action> GetActionsSatisfying(IntegerNocutPrecondition precondition)
        {
            return positiveResults.Select(res => res.Action);
        }
        
        protected override IEnumerable<HORNS.Action> GetActionsTowards(Variable<int> variable, int desiredValue)
        {
            if (variable.Value == desiredValue)
            {
                return new List<HORNS.Action>();
            }
            return (variable.Value < desiredValue ? positiveResults : negativeResults).Select(res => res.Action);
        }

        protected override void Register(IntegerAddResult result)
        {
            if (result.Term > 0)
            {
                positiveResults.Add(result);
            }
            else if (result.Term < 0)
            {
                negativeResults.Add(result);
            }
        }
    }

    class IntegerNocutVariable : Variable<int, IntegerAddResult, IntegerNocutSolver, IntegerNocutPrecondition>
    {
        public IntegerNocutVariable(int value = 0) : base(value)
        {

        }
    }
}
