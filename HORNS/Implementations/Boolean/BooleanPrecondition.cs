using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca wymaganie związane ze zmienną typu \texttt{bool}, które jest spełnione dla wartości równej określonej wartości.
    /// </summary>
    public class BooleanPrecondition : Precondition<bool, BooleanSolver>
    {
        /// <summary>
        /// Tworzy nowe wymaganie dla zmiennej typu \texttt{bool} o określonej wartości docelowej.
        /// </summary>
        /// <param name="value">Wartość docelowa wymagania.</param>
        public BooleanPrecondition(bool value) : base(value)
        {
        }

        private BooleanPrecondition(bool value, BooleanPrecondition other) : base(value, other)
        {
        }

        /// <summary>
        /// Tworzy nowe wymaganie typu \texttt{BooleanPrecondition} bedące kopią innego wymagania.
        /// </summary>
        /// <param name="precondition">Wymaganie do skopiowania.</param>
        public BooleanPrecondition(BooleanPrecondition precondition) : base(precondition)
        {
        }

        /// <summary>
        /// Łączy wymaganie z innym wymaganiem. Oba wymagania muszą być typu \texttt{BooleanPrecondition} i mieć tę samą wartość docelową.
        /// </summary>
        /// <param name="precondition">Wymaganie do połączenia.</param>
        /// <returns>Nowe wymaganie o wartości docelowej równej wartościom docelowym obu wymagań lub \texttt{null} w przypadku, gdy wymagań nie można połączyć.</returns>
        protected internal override Precondition Combine(Precondition precondition)
        {
            if (!(precondition is BooleanPrecondition boolPre) || Target != boolPre.Target)
            {
                return null;
            }
            return new BooleanPrecondition(Target, this);
        }

        /// <summary>
        /// Porównuje wymaganie z innym wymaganiem. Oba wymagania muszą być typu \texttt{BooleanPrecondition} i mieć tę samą wartość docelową.
        /// </summary>
        /// <param name="precondition">Wymaganie do porównania.</param>
        /// <returns>\texttt{true}, jeżeli obecne wymaganie jest w lepszym (spełnionym) stanie; \texttt{false} w przeciwnym wypadku lub jeśli wymagań nie można porównać.</returns>
        protected internal override bool IsBetterThan(Precondition precondition)
        {
            if (!(precondition is BooleanPrecondition boolPre) || Target != boolPre.Target)
            {
                return false;
            }
            // we can only be better if we're fulfilled
            return IsFulfilled();
        }

        /// <summary>
        /// Sprawdza, czy dana wartość spełnia wymaganie.
        /// Wartość spełnia wymaganie, jeżeli jest równa wartości docelowej.
        /// </summary>
        /// <param name="value">Wartość do sprawdzenia.</param>
        /// <param name="target">Wartość docelowa wymagania.</param>
        /// <returns>\texttt{true}, jeżeli wartość spełnia wymaganie.</returns>
        protected internal override bool IsFulfilled(bool value, bool target)
        {
            return value == target;
        }

        protected internal override bool IsFulfilledInState(bool value, bool target, bool state)
        {
            return state == target || value == target;
        }

        protected internal override bool GetDefault(bool target)
        {
            return !target;
        }

        /// <summary>
        /// Wykonuje kopię obiektu wymagania.
        /// </summary>
        /// <returns>Kopia wymagania.</returns>
        protected internal override Precondition Clone()
        {
            return new BooleanPrecondition(this);
        }
    }
}
