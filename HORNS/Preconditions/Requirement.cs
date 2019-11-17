using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class Requirement
    {
        protected internal abstract bool IsEqual(Requirement other);
        protected internal abstract IEnumerable<Action> GetActions();
        internal abstract bool IsFulfilled(VariableSet variablePatch);
    }
}
