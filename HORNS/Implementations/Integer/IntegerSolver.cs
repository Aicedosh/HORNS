using System;
using System.Collections.Generic;
using System.Linq;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca solver dla zmiennych typu int.
    /// </summary>
    public class IntegerSolver : VariableSolver<int, IntegerAddResult, IntegerPrecondition>
    {
        List<IntegerAddResult> positiveResults = new List<IntegerAddResult>();
        List<IntegerAddResult> negativeResults = new List<IntegerAddResult>();

        protected override IEnumerable<Action> GetActionsSatisfying(IntegerPrecondition precondition)
        {
            return (precondition.Direction == IntegerPrecondition.Condition.AtLeast ? positiveResults : negativeResults)
                .Select(res => res.Action);
        }

        protected override IEnumerable<Action> GetActionsTowards(Variable<int> variable, int desiredValue)
        {
            if (variable.Value == desiredValue)
            {
                return new List<Action>();
            }
            return (variable.Value < desiredValue ? positiveResults : negativeResults).Select(res => res.Action);
        }

        protected internal override void Register(IntegerAddResult result)
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
}
