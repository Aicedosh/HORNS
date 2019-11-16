using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class ActionResult
    {
        public Action Action { get; private protected set; }
        internal abstract Variable AbstractVariable { get; }
        internal abstract void Apply();
        internal abstract float GetCost(VariableSet variables);
    }
}
