using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca wymaganie związane ze zmienną typu \texttt{int}, które jest spełnione dla wartości zmiennej nie mniejszych od określonej stałej.
    /// Ten typ wymagania zakłada, że wymagana wartość zostanie zużyta w ramach rezultatu akcji.
    /// Wartość docelowa powinna być dodatnia.
    /// </summary>
    public class IntegerConsumePrecondition : Precondition<int>
    {
        /// <summary>
        /// Tworzy nowe wymaganie dla zmiennej typu \texttt{int} o określonej wartości wymaganej.
        /// </summary>
        /// <param name="value">Wartość wymagana.</param>
        public IntegerConsumePrecondition(int value)
            : base(value)
        {
        }

        private IntegerConsumePrecondition(int value, int state, IntegerConsumePrecondition other) : base(value, state, other)
        {
        }

        /// <summary>
        /// Tworzy nowe wymaganie typu \texttt{IntegerConsumePrecondition} bedące kopią innego wymagania.
        /// </summary>
        /// <param name="precondition">Wymaganie do skopiowania.</param>
        public IntegerConsumePrecondition(IntegerConsumePrecondition precondition) : base(precondition)
        {
        }
        
        /// <summary>
        /// Łączy wymaganie z innym wymaganiem. Oba wymagania muszą być typu \texttt{IntegerConsumePrecondition}.
        /// </summary>
        /// <param name="precondition">Wymaganie do połączenia.</param>
        /// <returns>Nowe wymaganie wartości wymaganej będącej sumą wartości wymaganych przez oba wymagania lub \texttt{null} w przypadku, gdy wymagań nie można połączyć.</returns>
        protected internal override Precondition Combine(Precondition precondition)
        {
            if (!(precondition is IntegerConsumePrecondition intPre))
            {
                return null;
            }
            return new IntegerConsumePrecondition(Target + intPre.Target, State + intPre.State, this);
        }

        /// <summary>
        /// Porównuje wymaganie z innym wymaganiem. Oba wymagania muszą być typu \texttt{IntegerConsumePrecondition} i tę samą wartość docelową.
        /// </summary>
        /// <param name="precondition">Wymaganie do porównania.</param>
        /// <returns>\texttt{true}, jeżeli obecne wymaganie jest w lepszym (bliższym wartości wymaganej) stanie; \texttt{false} w przeciwnym wypadku lub jeśli wymagań nie można porównać.</returns>
        protected internal override bool IsBetterThan(Precondition precondition)
        {
            if (!(precondition is IntegerConsumePrecondition intPre) || Target != intPre.Target)
            {
                return false;
            }
            return State > intPre.State;
        }

        /// <summary>
        /// Sprawdza, czy dana wartość spełnia wymaganie.
        /// Wartość spełnia wymaganie, jeżeli jest nie mniejsza od wartości wymaganej.
        /// </summary>
        /// <param name="value">Wartość do sprawdzenia.</param>
        /// <param name="target">Wartość docelowa wymagania.</param>
        /// <returns>\texttt{true}, jeżeli wartość spełnia wymaganie.</returns>
        protected internal override bool IsFulfilled(int value, int target)
        {
            return value >= target;
        }

        protected internal override bool IsFulfilledInState(int value, int target, int state)
        {
            return value + state >= target;
        }

        protected internal override int GetDefault()
        {
            return 0;
        }

        /// <summary>
        /// Wykonuje kopię obiektu wymagania.
        /// </summary>
        /// <returns>Kopia wymagania.</returns>
        protected internal override Precondition Clone()
        {
            return new IntegerConsumePrecondition(this);
        }
    }
}
