using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class VariableSolver<T>
    {
        internal Variable<T> Variable { get; set; }
        internal abstract IEnumerable<Action> GetActionsSatisfying(Precondition<T> precondition);
        protected internal abstract IEnumerable<Action> GetActionsTowards(Variable<T> variable, T desiredValue);
    }
}
