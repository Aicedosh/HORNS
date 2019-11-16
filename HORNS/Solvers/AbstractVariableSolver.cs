using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class VariableSolver<T>
    {
        internal abstract IEnumerable<Action> GetActionsSatisfying(Precondition<T> precondition);
    }
}
