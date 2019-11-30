using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class ActionResult
    {
        public Action Action { get; internal set; }
        internal abstract Variable AbstractVariable { get; }
        internal abstract void Apply();
        internal abstract void Apply(IdSet<Variable> variables);
        internal abstract float GetCost(IdSet<Variable> variables);
        internal abstract void SubtractFrom(PreconditionSet requirement);
    }
}
