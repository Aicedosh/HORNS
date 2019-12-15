using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HORNS
{
    public abstract class VariableSolver<T, RT, PT> : VariableSolver<T> where RT : ActionResult<T> where PT : Precondition<T>
    {
        protected internal abstract void Register(RT result);
        protected abstract IEnumerable<Action> GetActionsTowards(Variable<T> variable, T desiredValue);
        protected abstract IEnumerable<Action> GetActionsSatisfying(PT precondition);

        internal override IEnumerable<Action> GetActionsTowards(Variable<T> variable, T desiredValue, Agent agent)
        {
            return GetActionsTowards(variable, desiredValue).Where(a => agent.PossibleActions.Contains(a));
        }

        internal override IEnumerable<Action> GetActionsSatisfying(Precondition<T> precondition, Agent agent)
        {
            return GetActionsSatisfying(precondition as PT).Where(a=>agent.PossibleActions.Contains(a)); //TODO: Don't really like the cast
        }
    }
}
