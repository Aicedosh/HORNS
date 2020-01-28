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
    public class IntegerPrecondition : Precondition<int>
    {
        public bool Consumed { get; }

        /// <summary>
        /// Tworzy nowe wymaganie dla zmiennej typu \texttt{int} o określonej wartości wymaganej.
        /// </summary>
        /// <param name="target">Wartość wymagana.</param>
        /// <param name="consumed">Informacja, czy wymaganiu będzie towarzyszyć rezultat IntegerAddResult o wartości -target. </param>
        public IntegerPrecondition(int target, bool consumed)
            : base(target)
        {
            Consumed = consumed;
        }

        private IntegerPrecondition(int target, int state, IntegerPrecondition other) : base(target, state, other)
        {
            Consumed = other.Consumed;
        }

        /// <summary>
        /// Tworzy nowe wymaganie typu \texttt{IntegerConsumePrecondition} bedące kopią innego wymagania.
        /// </summary>
        /// <param name="precondition">Wymaganie do skopiowania.</param>
        public IntegerPrecondition(IntegerPrecondition precondition) : base(precondition)
        {
            Consumed = precondition.Consumed;
        }
        
        /// <summary>
        /// Łączy wymaganie z innym wymaganiem. Oba wymagania muszą być typu \texttt{IntegerConsumePrecondition} i dotyczyć tej samej zmiennej.
        /// </summary>
        /// <param name="precondition">Wymaganie do połączenia.</param>
        /// <returns>Nowe wymaganie wartości wymaganej będącej sumą wartości wymaganych przez oba wymagania lub \texttt{null} w przypadku, gdy wymagań nie można połączyć.</returns>
        protected internal override Precondition Combine(Precondition precondition)
        {
            if (!(precondition is IntegerPrecondition intPre)
                || Variable.Id != intPre.Variable.Id)
            {
                return null;
            }
            int target = intPre.Consumed ? Target : Math.Max(Target, intPre.Target);
            return new IntegerPrecondition(target, State + intPre.State, this);
        }

        /// <summary>
        /// Porównuje wymaganie z innym wymaganiem. Oba wymagania muszą być typu \texttt{IntegerConsumePrecondition}, dotyczyć tej samej zmiennej i mieć tę samą wartość docelową.
        /// </summary>
        /// <param name="precondition">Wymaganie do porównania.</param>
        /// <returns>\texttt{true}, jeżeli obecne wymaganie jest w lepszym (bliższym wartości wymaganej) stanie; \texttt{false} w przeciwnym wypadku lub jeśli wymagań nie można porównać.</returns>
        protected internal override ComparisonResult IsBetterThan(Precondition precondition)
        {
            if (!(precondition is IntegerPrecondition intPre)
                || Variable.Id != intPre.Variable.Id
                || Target != intPre.Target)
            {
                return ComparisonResult.NotComparable;
            }
            return State > intPre.State ? ComparisonResult.Better : ComparisonResult.EqualWorse;
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
            return new IntegerPrecondition(this);
        }
    }
}
