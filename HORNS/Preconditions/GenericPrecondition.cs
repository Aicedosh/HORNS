using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Abstrakcyjna klasa bazowa dla wymagań dla zmiennych typu T.
    /// </summary>
    /// <typeparam name="T">Typ danych przechowywanych w zmiennej związanej z wymaganiem.</typeparam>
    public abstract class Precondition<T> : Precondition
    {
        /// <summary>
        /// Zmienna związana z wymaganiem.
        /// </summary>
        protected internal Variable<T> Variable { get; internal set; }
        /// <summary>
        /// Wartość docelowa wymagania.
        /// </summary>
        public T Target { get; }
        // current state
        internal T State { get; set; }

        /// <summary>
        /// Tworzy nowe wymaganie o określonej wartości docelowej.
        /// </summary>
        /// <param name="value">Wartość docelowa wymagania.</param>
        public Precondition(T target)
        {
            Target = target;
            State = target;
        }

        /// <summary>
        /// Tworzy nowe wymaganie bedące kopią innego wymagania.
        /// </summary>
        /// <param name="precondition">Wymaganie do skopiowania.</param>
        public Precondition(Precondition<T> precondition)
        {
            Target = precondition.Target;
            State = precondition.State;
            Variable = precondition.Variable;
        }

        internal override Variable GetVariable()
        {
            return Variable;
        }

        /// <summary>
        /// Sprawdza, czy dana wartość spełnia wymaganie.
        /// </summary>
        /// <param name="value">Wartość do sprawdzenia.</param>
        /// <returns>\texttt{true}, jeżeli wartość spełnia wymaganie.</returns>
        protected internal abstract bool IsFulfilled(T value);
        /// <summary>
        /// Sprawdza, czy wymaganie dążące do danej wartości można uznać za spełnione.
        /// </summary>
        /// <param name="value">Wartość do sprawdzenia.</param>
        /// <returns>\texttt{true}, jeżeli dla danej wartości docelowej wymaganie jest spełnione.</returns>
        protected internal abstract bool IsZeroed(T value);

        internal override bool IsFulfilled()
        {
            return IsZeroed(State);
        }

        internal override bool IsFulfilledByWorld()
        {
            return IsFulfilled(Variable.Value);
        }

        internal override bool IsFulfilledBy(IdSet<Variable> variables)
        {
            Variable variable = Variable;
            if (variables != null)
            {
                variables.TryGet(ref variable);
            }
            return IsFulfilled((variable as Variable<T>).Value);
        }
    }
}
