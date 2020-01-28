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
        /// Tworzy nowy rezultat dla zmiennej typu T będący kopią innego rezultatu.
        /// </summary>
        /// <param name="other">Rezultat do skopiowania.</param>
        protected ActionResult(ActionResult<T> other) : base(other)
        {
            Variable = other.Variable;
        }

        /// <summary>
        /// Zwraca wartość końcową rezultatu dla danej wartości początkowej.
        /// </summary>
        /// <param name="value">Wartość początkowa.</param>
        /// <returns>Wartość końcowa po zastosowaniu rezultatu.</returns>
        protected internal abstract T GetResultValue(T value);

        /// <summary>
        /// Sprawdza, czy możliwe jest zastosowanie rezultatu do stanu danego wymagania.
        /// </summary>
        /// <param name="precondition">Wymaganie do sprawdzenia.</param>
        /// <returns>true, jeżeli rezultat można zastosować; false w przeciwnym wypadku.</returns>
        protected internal abstract bool CanApply(Precondition<T> precondition);

        internal override float GetCost(IdSet<Variable> variables, Agent agent)
        {
            Variable currentVariable = Variable;
            if (variables != null)
            {
                variables.TryGet(ref currentVariable);
            }
            Variable<T> curr = currentVariable as Variable<T>;

            IEvaluable<T> evaluator = curr;
            if(agent.NeedsInternal.Contains(Variable.Id))
            {
                evaluator = agent.NeedsInternal[Variable.Id] as Need<T>;
            }

            return evaluator.Evaluate(curr.Value) - evaluator.Evaluate(GetResultValue(curr.Value));
        }

        internal override void Apply()
        {
            Variable.Value = GetResultValue(Variable.Value);
        }

        internal override void Apply(Variable variable)
        {
            Variable<T> typedVar = variable as Variable<T>;
            typedVar.Value = GetResultValue(typedVar.Value);
        }

        internal override void Apply(IdSet<Variable> variables)
        {
            if (!variables.Contains(Variable.Id))
            {
                variables.Add(Variable.GetCopy());
            }
            var variable = variables[Variable.Id] as Variable<T>;
            variable.Value = GetResultValue(variable.Value);
        }

        internal override bool Apply(PreconditionSet preconditions)
        {
            if (!preconditions.Contains(Variable.Id))
            {
                return true;
            }
            var pre = preconditions[Variable.Id] as Precondition<T>;
            if (!CanApply(pre))
            {
                return false;
            }
            preconditions.Replace(pre.Apply(this));
            return true;
        }

        /// <summary>
        /// Wykonuje kopię danego rezultatu.
        /// </summary>
        /// <returns>Nowy rezultat będący kopią obecnego.</returns>
        protected internal abstract ActionResult<T> Clone();
    }
}
