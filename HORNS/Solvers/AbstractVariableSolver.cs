using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class VariableSolver<T>
    {
        public abstract IEnumerable<Action> GetActions(Variable<T> variable, T goalValue);
    }
}
