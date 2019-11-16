using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class ActionResult<T> : ActionResult
    {
        protected Variable<T> Variable { get; private set; }
        internal override Variable AbstractVariable => Variable;

        private protected ActionResult(Variable<T> variable)
        {
            Variable = variable;
        }

        protected internal abstract T GetResultValue(Variable<T> variable);

        internal override float GetCost(VariableSet variables)
        {
            Variable currentVariable;
            if (!variables.TryGet(Variable, out currentVariable))
            {
                currentVariable = Variable;
            }
            Variable<T> curr = currentVariable as Variable<T>; //TODO: Can we remove this cast?
            return curr.Evaluate(GetResultValue(curr)) - curr.Evaluate(curr.Value);
        }
    }
}
