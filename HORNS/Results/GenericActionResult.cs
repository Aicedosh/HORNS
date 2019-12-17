using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Abstrakcyjna klasa bazowa dla rezultatów dla zmiennych typu T.
    /// </summary>
    /// <typeparam name="T">Typ danych przechowywanych w zmiennej związanej z rezultatem.</typeparam>
    public abstract class ActionResult<T> : ActionResult
    {
        /// <summary>
        /// Zmienna związana z wymaganiem.
        /// </summary>
        protected internal Variable<T> Variable { get; internal set; }
        internal override Variable AbstractVariable => Variable;

        private protected ActionResult()
        {
        }

        /// <summary>
        /// Zwraca wartość końcową rezultatu dla wartości początkowej równą wartości danej zmiennej.
        /// </summary>
        /// <param name="variable">Zmienna o wartości początkowej.</param>
        /// <returns>Wartość końcowa rezultatu.</returns>
        protected internal abstract T GetResultValue(Variable<T> variable);

        internal override float GetCost(IdSet<Variable> variables, Agent agent)
        {
            Variable currentVariable = Variable;
            if (variables != null)
            {
                variables.TryGet(ref currentVariable);
            }
            Variable<T> curr = currentVariable as Variable<T>; //TODO: Can we remove this cast?

            IEvaluable<T> evaluator = curr;
            if(agent.NeedsInternal.Contains(Variable.Id))
            {
                evaluator = agent.NeedsInternal[Variable.Id] as Need<T>;
            }

            return evaluator.Evaluate(curr.Value) - evaluator.Evaluate(GetResultValue(curr));
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
