using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Abstrakcyjna klasa bazowa dla wszystkich rezultatów.
    /// </summary>
    public abstract class ActionResult
    {
        internal Action Action { get; set; }

        internal abstract Variable AbstractVariable { get; }
        internal abstract void Apply();
        internal abstract void Apply(Variable variable);
        internal abstract void Apply(IdSet<Variable> variables);
        internal abstract float GetCost(IdSet<Variable> variables, Agent agent);
        internal abstract void SubtractFrom(PreconditionSet requirement);
    }
}
