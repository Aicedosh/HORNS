using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca wymaganie związane ze zmienną typu int.
    /// </summary>
    public class IntegerPrecondition : Precondition<int, IntegerSolver>
    {
        /// <summary>
        /// Typ wyliczeniowy reprezentujący kierunek porównania.
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
        /// Tworzy nowe wymaganie typu int o określonej wartości docelowej i warunku do spełnienia.
        /// </summary>
        /// <param name="value">Wartość docelowa wymagania.</param>
        /// <param name="direction">Warunek, który musi być spełniony względem wartości docelowej.</param>
        public IntegerPrecondition(int value, Condition direction)
            : base(value)
        {
            Direction = direction;
        }

        private IntegerPrecondition(int value, IntegerPrecondition other) : base(value, other)
        {
        }

        /// <summary>
        /// Tworzy nowe wymaganie typu int bedące kopią innego wymagania.
        /// </summary>
        /// <param name="precondition">Wymaganie do skopiowania.</param>
        public IntegerPrecondition(IntegerPrecondition precondition) : base(precondition)
        {
            Direction = precondition.Direction;
        }

        /// <summary>
        /// Łączy wymaganie z innym wymaganiem.
        /// </summary>
        /// <param name="precondition">Wymaganie do połączenia.</param>
        /// <returns>Nowe wymaganie będące wynikiem połączenia.</returns>
        protected internal override Precondition Combine(Precondition precondition)
        {
            if (!(precondition is IntegerPrecondition intPre) || Direction != intPre.Direction)
            {
                return null;
            }
            return new IntegerPrecondition(Value + intPre.Value, this);
        }

        /// <summary>
        /// Porównuje wymaganie z innym wymaganiem.
        /// </summary>
        /// <param name="precondition">Wymaganie do porównania.</param>
        /// <returns>\texttt{true}, jeżeli \texttt{other} jest w takim samym lub gorszym stanie; \texttt{false} w przeciwnym wypadku.</returns>
        protected internal override bool IsEqualOrWorse(Precondition precondition)
        {
            if (!(precondition is IntegerPrecondition intPre) || Direction != intPre.Direction || Value != intPre.Value)
            {
                return false;
            }
            return Variable.Value >= intPre.Variable.Value;
        }

        /// <summary>
        /// Odejmuje rezultat akcji od wymagania.
        /// </summary>
        /// <param name="actionResult">Rezultat do odjęcia.</param>
        /// <returns>Nowe wymaganie będące wynikiem odjęcia rezultatu.</returns>
        protected internal override Precondition Subtract(ActionResult actionResult)
        {
            var addRes = actionResult as IntegerAddResult;
            int newVal = Value + addRes.Term * (Direction == Condition.AtMost ? 1 : -1);

            return new IntegerPrecondition(newVal, this);
        }

        /// <summary>
        /// Sprawdza, czy dana wartość spełnia wymaganie.
        /// </summary>
        /// <param name="value">Wartość do sprawdzenia.</param>
        /// <returns>\texttt{true}, jeżeli wartość spełnia wymaganie.</returns>
        protected internal override bool IsFulfilled(int value)
        {
            return Direction == Condition.AtLeast ? value >= Value : value <= Value;
        }

        /// <summary>
        /// Sprawdza, czy wymaganie dążące do danej wartości można uznać za spełnione.
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
