using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HORNS
{
    public abstract class ActionResult
    {
        internal Action Action { get; set; }

        internal abstract Variable AbstractVariable { get; }
        internal abstract void Apply();
        internal abstract void Apply(Variable variable);
        internal abstract void Apply(IdSet<Variable> variables);
        internal abstract float GetCost(IdSet<Variable> variables);
        internal abstract void SubtractFrom(PreconditionSet requirement);
    }
}
