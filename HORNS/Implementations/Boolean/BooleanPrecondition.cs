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
            if (!(precondition is BooleanPrecondition boolPre) || Value != boolPre.Value)
            {
                return null;
            }
            return new BooleanPrecondition(Value, this);
        }

        /// <summary>
        /// Porównuje wymaganie z innym wymaganiem. Oba wymagania muszą być typu \texttt{BooleanPrecondition} i mieć tę samą wartość docelową.
        /// </summary>
        /// <param name="precondition">Wymaganie do porównania.</param>
        /// <returns>\texttt{true}, jeżeli \texttt{other} jest w takim samym lub gorszym stanie; \texttt{false} w przeciwnym wypadku lub jeśli wymagań nie można porównać.</returns>
        protected internal override bool IsEqualOrWorse(Precondition precondition)
        {
            if (!(precondition is BooleanPrecondition boolPre) || Value != boolPre.Value)
            {
                return false;
            }
            // pre will always be unfulfilled, which means that we're either equal or better
            return Variable.Value == boolPre.Variable.Value;
        }

        /// <summary>
        /// Odejmuje rezultat akcji od wymagania. Rezultat musi być typu \texttt{BooleanResult}.
        /// </summary>
        /// <param name="actionResult">Rezultat do odjęcia.</param>
        /// <returns>Nowe wymaganie z wartością docelową będącą odwrotnością wartości końcowej rezultatu.</returns>
        protected internal override Precondition Subtract(ActionResult actionResult)
        {
            return new BooleanPrecondition(!(actionResult as BooleanResult).EndValue, this);
        }

        /// <summary>
        /// Sprawdza, czy dana wartość spełnia wymaganie.
        /// Wartość spełnia wymaganie, jeżeli jest równa wartości docelowej.
        /// </summary>
        /// <param name="value">Wartość do sprawdzenia.</param>
        /// <returns>\texttt{true}, jeżeli wartość spełnia wymaganie.</returns>
        protected internal override bool IsFulfilled(bool value)
        {
            return value == Value;
        }

        // TODO: this is temporary
        /// <summary>
        /// Sprawdza, czy wymaganie dążące do danej wartości można uznać za spełnione.
        /// Wymaganie można uznać za spełnione, jeżeli wartość pozostała do spełnienia jest przeciwna do docelowej.
        /// </summary>
        /// <param name="value">Wartość do sprawdzenia.</param>
        /// <returns>\texttt{true}, jeżeli dla danej wartości docelowej wymaganie jest spełnione.</returns>
        protected internal override bool IsZeroed(bool value)
        {
            return value != Value;
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
