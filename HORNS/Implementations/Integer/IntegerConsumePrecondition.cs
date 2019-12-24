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
    public class IntegerConsumePrecondition : Precondition<int, IntegerConsumeSolver>
    {
        /// <summary>
        /// Tworzy nowe wymaganie dla zmiennej typu \texttt{int} o określonej wartości wymaganej.
        /// </summary>
        /// <param name="value">Wartość wymagana.</param>
        public IntegerConsumePrecondition(int value)
            : base(value)
        {
        }

        private IntegerConsumePrecondition(int value, IntegerConsumePrecondition other) : base(value, other)
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
            return new IntegerConsumePrecondition(Value + intPre.Value, this);
        }

        /// <summary>
        /// Porównuje wymaganie z innym wymaganiem. Oba wymagania muszą być typu \texttt{IntegerConsumePrecondition} i tę samą wartość docelową.
        /// </summary>
        /// <param name="precondition">Wymaganie do porównania.</param>
        /// <returns>\texttt{true}, jeżeli \texttt{precondition} jest w takim samym lub gorszym (bardziej odległym od wartości wymaganej) stanie; \texttt{false} w przeciwnym wypadku lub jeśli wymagań nie można porównać.</returns>
        protected internal override bool IsEqualOrWorse(Precondition precondition)
        {
            if (!(precondition is IntegerConsumePrecondition intPre) || Value != intPre.Value)
            {
                return false;
            }
            return Variable.Value >= intPre.Variable.Value;
        }

        /// <summary>
        /// Odejmuje rezultat akcji od wymagania. Rezultat musi być typu \texttt{IntegerAddResult}.
        /// </summary>
        /// <param name="actionResult">Rezultat do odjęcia.</param>
        /// <returns>Nowe wymaganie z wartością wymaganą zmienioną w zależności od wartości składnika rezultatu.</returns>
        protected internal override Precondition Subtract(ActionResult actionResult)
        {
            var addRes = actionResult as IntegerAddResult;
            int newVal = Value - addRes.Term;

            return new IntegerConsumePrecondition(newVal, this);
        }

        /// <summary>
        /// Sprawdza, czy dana wartość spełnia wymaganie.
        /// Wartość spełnia wymaganie, jeżeli jest nie mniejsza od wartości wymaganej.
        /// </summary>
        /// <param name="value">Wartość do sprawdzenia.</param>
        /// <returns>\texttt{true}, jeżeli wartość spełnia wymaganie.</returns>
        protected internal override bool IsFulfilled(int value)
        {
            return value >= Value;
        }

        /// <summary>
        /// Sprawdza, czy wymaganie dążące do danej wartości można uznać za spełnione.
        /// Wymaganie można uznać za spełnione, gdy wartość pozostała do spełnienia jest niedodatnia.
        /// </summary>
        /// <param name="value">Wartość do sprawdzenia.</param>
        /// <returns>\texttt{true}, jeżeli dla danej wartości wymaganej wymaganie jest spełnione.</returns>
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
            return new IntegerConsumePrecondition(this);
        }
    }
}
