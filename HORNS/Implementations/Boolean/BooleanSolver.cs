using System;
using System.Collections.Generic;
using System.Linq;

namespace HORNS
{
    public class BooleanSolver : VariableSolver<bool, BooleanResult, BooleanPrecondition>
    {
        List<BooleanResult> trueResults = new List<BooleanResult>();
        List<BooleanResult> falseResults = new List<BooleanResult>();
        protected override IEnumerable<Action> GetActionsSatisfying(BooleanPrecondition precondition)
        {
            return GetActions(precondition.Value);
        }

        protected override IEnumerable<Action> GetActionsTowards(Variable<bool> variable, bool desiredValue)
        {
            return GetActions(desiredValue);
        }

        private IEnumerable<Action> GetActions(bool value)
        {
            return (value ? trueResults : falseResults).Select(res => res.Action);
        }

        protected internal override void Register(BooleanResult result)
        {
            var destList = result.EndValue ? trueResults : falseResults;
            destList.Add(result);
        }
    }
}
