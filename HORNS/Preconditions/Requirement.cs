using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class Requirement
    {
        protected internal abstract bool IsEqual(Requirement other);
        protected internal abstract IEnumerable<Action> GetActions();
        internal bool Fulfilled { get; private protected set; } = false;    // TODO: property + methods = dis ugly
        internal abstract bool IsFulfilled();
        internal abstract bool IsFulfilled(VariableSet variables);
        internal abstract Requirement Clone();
    }
}
