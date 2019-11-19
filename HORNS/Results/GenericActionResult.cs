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

        internal override void SubtractFrom(RequirementSet requirements)
        {
            if (!requirements.Contains(Variable.Id)) return;
            Requirement req = requirements[Variable.Id];
            Requirement newReq = req.Subtract(this);
            requirements.Replace(newReq);
        }

        //internal override void Apply(VariableSet variables)
        //{
        //    Variable currentVariable = Variable;
        //    if (!variables.TryGet(ref currentVariable))
        //    {
        //        currentVariable = Variable.GetCopy();
        //        variables.Add(currentVariable);
        //    }
        //    Variable<T> curr = currentVariable as Variable<T>;
        //    curr.Value = GetResultValue(curr);
        //}
    }
}
