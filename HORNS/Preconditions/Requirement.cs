using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class Requirement
    {
        protected internal abstract bool IsEqual(Requirement other);
        protected internal abstract IEnumerable<Action> GetActions();
        internal bool Fulfilled { get; private set; } = false;
        internal abstract bool IsFulfilled(VariableSet variablePatch);
        internal abstract Requirement Clone();
    }
}
