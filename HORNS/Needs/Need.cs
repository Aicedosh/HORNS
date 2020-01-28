using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca potrzeby dotyczące zmiennych typu T.
    /// </summary>
    /// <typeparam name="T">Typ danych przechowywanych w zmiennej związanej z potrzebą.</typeparam>
    public class Need<T> : INeedInternal, IIdentifiable, IEvaluable<T>
    {
        private readonly Func<T, float> evaluation;
        private readonly Func<T, bool> isSatisfied;

        /// <summary>
        /// Wartość zmiennej związanej z potrzebą.
        /// </summary>
        public T Value { get => Variable.Value; }
        internal Variable<T> Variable { get; private set; }
        /// <summary>
        /// Docelowa wartość potrzeby.
        /// </summary>
        public T Desired { get; private set; }

        internal VariableSolver<T> GenericSolver => Variable.GenericSolver;

        private Need(Need<T> other)
        {
            Variable = other.Variable;
            Desired = other.Desired;
            evaluation = other.evaluation;
            isSatisfied = other.isSatisfied;
        }

        /// <summary>
        /// Tworzy nową potrzebę powiązaną z określoną zmienną.
        /// </summary>
        /// <param name="variable">Zmienna, której dotyczy potrzeba.</param>
        /// <param name="desired">Docelowa wartość zmiennej.</param>
        /// <param name="evaluation">Funkcja wyznaczająca ocenę potrzeby dla konkretnej wartości zmiennej.</param>
        /// <param name="isSatisfied">Funkcja zwracająca informację, czy potrzeba jest spełniona dla danej wartości powiązanej zmiennej.</param>
        public Need(Variable<T> variable, T desired, Func<T, float> evaluation, Func<T, bool> isSatisfied = null)
        {
            Variable = variable;
            Desired = desired;
            this.evaluation = evaluation;
            this.isSatisfied = isSatisfied ?? (v => v.Equals(Desired));
        }

        /// <summary>
        /// Oblicza ocenę potrzeby dla danej wartości.
        /// </summary>
        /// <param name="value">Wartość, dla której należy wyznaczyć ocenę.</param>
        /// <returns>Ocena dla danej wartości.</returns>
        public float Evaluate(T value)
        {
            return evaluation(value);
        }

        /// <summary>
        /// Oblicza ocenę obecnego stanu potrzeby.
        /// </summary>
        /// <returns>Ocena stanu potrzeby.</returns>
        public float Evaluate()
        {
            return Evaluate(Value);
        }

        internal float EvaluateFor(Variable variable)
        {
            return Evaluate((variable as Variable<T>).Value);
        }

        internal float EvaluateFor(IdSet<Variable> variables)
        {
            Variable variable = Variable;
            if (variables != null)
            {
                variables.TryGet(ref variable);
            }
            return EvaluateFor(variable);
        }

        IEnumerable<Action> GetActionsTowards(Agent agent)
        {
            return GenericSolver.GetActionsTowards(Variable, Desired, agent);
        }

        /// <summary>
        /// Sprawdza, czy potrzeba jest w stanie zaspokojonym.
        /// </summary>
        /// <returns>true, jeżeli potrzeba jest zaspokojona; false w przeciwnym wypadku.</returns>
        public bool IsSatisfied()
        {
            return isSatisfied(Value);
        }

        internal bool IsSatisfied(IdSet<Variable> variables)
        {
            Variable variable = Variable;
            if (variables != null)
            {
                variables.TryGet(ref variable);
            }
            return isSatisfied((variable as Variable<T>).Value);
        }

        Variable GetVariable()
        {
            return Variable;
        }

        float INeedInternal.EvaluateFor(Variable variable) => EvaluateFor(variable);
        float INeedInternal.EvaluateFor(IdSet<Variable> variables) => EvaluateFor(variables);
        bool INeedInternal.IsSatisfied(IdSet<Variable> variables) => IsSatisfied(variables);
        Variable INeedInternal.GetVariable() => GetVariable();
        IEnumerable<Action> INeedInternal.GetActionsTowards(Agent agent) => GetActionsTowards(agent);

        int IIdentifiable.Id => Variable.Id;
        IIdentifiable IIdentifiable.GetCopy()
        {
            throw new NotImplementedException();
        }
    }
}
