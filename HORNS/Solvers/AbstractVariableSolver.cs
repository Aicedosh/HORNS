using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class VariableSolver<T>
    {
        protected internal abstract IEnumerable<Action> GetActions(Variable<T> variable, T goalValue); //goalValue -> requirement
    }
}
