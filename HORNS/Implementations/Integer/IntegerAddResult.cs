using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca rezultat związany ze zmienną typu int, który powoduje zwiększenie lub zmniejszenie wartości zmiennej o stałą wartość.
    /// </summary>
    public class IntegerAddResult : ActionResult<int>
    {
        /// <summary>
        /// Składnik (dodatni lub ujemny) dodawany do wartości zmiennej w wyniku wykonania rezultatu.
        /// </summary>
        public int Term { get; }

        /// <summary>
        /// Tworzy nowy rezultat dla zmiennej typu int, którego wynikiem jest dodanie danej wartości do wartości zmiennej.
        /// </summary>
        /// <param name="term">Składnik (dodatni lub ujemny) dodawany do wartości zmiennej.</param>
        public IntegerAddResult(int term)
        {
            Term = term;
        }

        private IntegerAddResult(IntegerAddResult other) : base(other)
        {
            Term = other.Term;
        }

        /// <summary>
        /// Zwraca wartość końcową rezultatu dla danej wartości początkowej.
        /// Wartość końcowa jest równa sumie wartości początkowej oraz Term.
        /// </summary>
        /// <param name="value">Wartość początkowa.</param>
        /// <returns>Wartość końcowa po zastosowaniu rezultatu.</returns>
        protected internal override int GetResultValue(int value)
        {
            return value + Term;
        }

        /// <summary>
        /// Tworzy kopię rezultatu.
        /// </summary>
        /// <returns>Nowy rezultat będący kopią obecnego.</returns>
        protected internal override ActionResult<int> Clone()
        {
            return new IntegerAddResult(this);
        }

        /// <summary>
        /// Sprawdza, czy możliwe jest zastosowanie rezultatu do stanu danego wymagania.
        /// </summary>
        /// <param name="precondition">Wymaganie do sprawdzenia.</param>
        /// <returns>true, jeżeli rezultat można zastosować; false w przeciwnym wypadku.</returns>
        protected internal override bool CanApply(Precondition<int> precondition)
        {
            return true;
        }
    }
}
