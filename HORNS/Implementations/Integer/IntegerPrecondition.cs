using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca wymaganie związane ze zmienną typu \texttt{int}, które jest spełnione dla wartości zmiennej nie większych/nie mniejszych od określonej stałej.
    /// Wartość docelowa powinna być dodatnia.
    /// </summary>
    public class IntegerPrecondition : Precondition<int, IntegerSolver>
    {
        /// <summary>
        /// Typ wyliczeniowy reprezentujący kierunek porównania wartości zmiennej z wartością docelową (AtLeast lub AtMost).
        /// </summary>
        public enum Condition
        {
            AtLeast, AtMost
        }
        /// <summary>
        /// Kierunek porównania obecnej wartości z wartością docelową.
        /// </summary>
        public Condition Direction { get; }

        /// <summary>
        /// Tworzy nowe wymaganie dla zmiennej typu \texttt{int} o określonej wartości docelowej i warunku do spełnienia względem tej wartości.
        /// </summary>
        /// <param name="value">Wartość docelowa wymagania.</param>
        /// <param name="direction">Warunek, który musi być spełniony względem wartości docelowej (nie większy/nie mniejszy).</param>
        public IntegerPrecondition(int value, Condition direction)
            : base(value)
        {
            Direction = direction;
        }

        private IntegerPrecondition(int value, IntegerPrecondition other) : base(value, other)
        {
        }

        /// <summary>
        /// Tworzy nowe wymaganie typu \texttt{IntegerPrecondition} bedące kopią innego wymagania.
        /// </summary>
        /// <param name="precondition">Wymaganie do skopiowania.</param>
        public IntegerPrecondition(IntegerPrecondition precondition) : base(precondition)
        {
            Direction = precondition.Direction;
        }

        // TODO: [M] fix!
        /// <summary>
        /// Łączy wymaganie z innym wymaganiem. Oba wymagania muszą być typu \texttt{IntegerPrecondition} i mieć ten sam kierunek porównania.
        /// </summary>
        /// <param name="precondition">Wymaganie do połączenia.</param>
        /// <returns>Nowe wymaganie o kierunku porównania zgodnym z kierunkiem porównania wymagań składowych i wartości docelowej spełniającej oba wymagania lub \texttt{null} w przypadku, gdy wymagań nie można połączyć.</returns>
        protected internal override Precondition Combine(Precondition precondition)
        {
            if (!(precondition is IntegerPrecondition intPre) || Direction != intPre.Direction)
            {
                return null;
            }
            return new IntegerPrecondition(Value + intPre.Value, this);
        }

        /// <summary>
        /// Porównuje wymaganie z innym wymaganiem. Oba wymagania muszą być typu \texttt{IntegerPrecondition}, mieć ten sam kierunek porównania i tę samą wartość docelową.
        /// </summary>
        /// <param name="precondition">Wymaganie do porównania.</param>
        /// <returns>\texttt{true}, jeżeli \texttt{precondition} jest w takim samym lub gorszym (bardziej odległym od wartości docelowej) stanie; \texttt{false} w przeciwnym wypadku lub jeśli wymagań nie można porównać.</returns>
        protected internal override bool IsEqualOrWorse(Precondition precondition)
        {
            if (!(precondition is IntegerPrecondition intPre) || Direction != intPre.Direction || Value != intPre.Value)
            {
                return false;
            }
            return Variable.Value >= intPre.Variable.Value;
        }

        /// <summary>
        /// Odejmuje rezultat akcji od wymagania. Rezultat musi być typu \texttt{IntegerAddResult}.
        /// </summary>
        /// <param name="actionResult">Rezultat do odjęcia.</param>
        /// <returns>Nowe wymaganie z wartością docelową zmienioną w zależności od wartości składnika rezultatu.</returns>
        protected internal override Precondition Subtract(ActionResult actionResult)
        {
            var addRes = actionResult as IntegerAddResult;
            int newVal = Value + addRes.Term * (Direction == Condition.AtMost ? 1 : -1);

            return new IntegerPrecondition(newVal, this);
        }

        /// <summary>
        /// Sprawdza, czy dana wartość spełnia wymaganie.
        /// Wartość spełnia wymaganie, jeżeli jest nie większa (dla kierunku AtMost) lub nie mniejsza (dla kierunku AtLeast) od wartości docelowej.
        /// </summary>
        /// <param name="value">Wartość do sprawdzenia.</param>
        /// <returns>\texttt{true}, jeżeli wartość spełnia wymaganie.</returns>
        protected internal override bool IsFulfilled(int value)
        {
            return Direction == Condition.AtLeast ? value >= Value : value <= Value;
        }

        /// <summary>
        /// Sprawdza, czy wymaganie dążące do danej wartości można uznać za spełnione.
        /// Wymaganie można uznać za spełnione, gdy wartość pozostała do spełnienia jest niedodatnia.
        /// </summary>
        /// <param name="value">Wartość do sprawdzenia.</param>
        /// <returns>\texttt{true}, jeżeli dla danej wartości docelowej wymaganie jest spełnione.</returns>
        protected internal override bool IsZeroed(int value)
        {
            return value <= 0;
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
