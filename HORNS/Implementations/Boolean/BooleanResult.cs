using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca rezultat związany ze zmienną typu bool, który powoduje przyjęcie przez zmienną określonej wartości.
    /// </summary>
    public class BooleanResult : ActionResult<bool>
    {
        /// <summary>
        /// Wartość, jaką ma przyjąć zmienna po wykonaniu rezultatu.
        /// </summary>
        public bool EndValue { get; }

        /// <summary>
        /// Tworzy nowy rezultat związany dla zmiennej typu bool, którego wynikiem jest zmiana wartości zmiennej na określoną wartość.
        /// </summary>
        /// <param name="endValue">Wartość końcowa rezultatu.</param>
        public BooleanResult(bool endValue)
        {
            EndValue = endValue;
        }

        private BooleanResult(BooleanResult other) : base(other)
        {
            EndValue = other.EndValue;
        }

        /// <summary>
        /// Zwraca wartość końcową rezultatu dla danej wartości początkowej.
        /// Wartość końcowa jest równa EndValue.
        /// </summary>
        /// <param name="value">Wartość początkowa.</param>
        /// <returns>Wartość końcowa po zastosowaniu rezultatu.</returns>
        protected internal override bool GetResultValue(bool value)
        {
            return EndValue;
        }

        /// <summary>
        /// Tworzy kopię rezultatu.
        /// </summary>
        /// <returns>Nowy rezultat będący kopią obecnego.</returns>
        protected internal override ActionResult<bool> Clone()
        {
            return new BooleanResult(this);
        }

        /// <summary>
        /// Sprawdza, czy możliwe jest zastosowanie rezultatu do stanu danego wymagania.
        /// </summary>
        /// <param name="precondition">Wymaganie do sprawdzenia.</param>
        /// <returns>true, jeżeli rezultat można zastosować; false w przeciwnym wypadku.</returns>
        protected internal override bool CanApply(Precondition<bool> precondition)
        {
            if (EndValue != precondition.Target)
            {
                return false;
            }
            return true;
        }
    }
}
