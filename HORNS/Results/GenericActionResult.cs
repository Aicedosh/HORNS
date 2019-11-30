using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class ActionResult<T> : ActionResult
    {
        protected internal Variable<T> Variable { get; internal set; }
        internal override Variable AbstractVariable => Variable;

        private protected ActionResult()
        {
        }

        protected internal abstract T GetResultValue(Variable<T> variable);

        internal override float GetCost(IdSet<Variable> variables)
        {
            Variable currentVariable = Variable;
            if (variables != null)
            {
                variables.TryGet(ref currentVariable);
            }
            Variable<T> curr = currentVariable as Variable<T>; //TODO: Can we remove this cast?
            return curr.Evaluate(GetResultValue(curr)) - curr.Evaluate(curr.Value);
        }

        internal override void Apply()
        {
            Variable.Value = GetResultValue(Variable);
        }

        internal override void Apply(Variable variable)
        {
            Variable<T> typedVar = variable as Variable<T>;
            typedVar.Value = GetResultValue(typedVar);
        }

        internal override void Apply(IdSet<Variable> variables)
        {
            if (!variables.Contains(Variable.Id))
            {
                variables.Add(Variable.GetCopy());
            }
            var variable = variables[Variable.Id] as Variable<T>;
            variable.Value = GetResultValue(variable);
        }

        internal override void SubtractFrom(PreconditionSet preconditions)
        {
            if (!preconditions.Contains(Variable.Id)) return;
            Precondition pre = preconditions[Variable.Id];
            Precondition newPre = pre.Subtract(this);
            preconditions.Replace(newPre);
        }
    }
}
