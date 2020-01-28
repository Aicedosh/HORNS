using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca wymaganie związane ze zmienną typu bool, które jest spełnione dla wartości równej określonej wartości.
    /// </summary>
    public class BooleanPrecondition : Precondition<bool>
    {
        /// <summary>
        /// Tworzy nowe wymaganie dla zmiennej typu bool o określonej wartości docelowej.
        /// </summary>
        /// <param name="target">Wartość docelowa wymagania.</param>
        public BooleanPrecondition(bool target) : base(target)
        {
        }

        private BooleanPrecondition(bool target, bool state, BooleanPrecondition other) : base(target, state, other)
        {
        }

        /// <summary>
        /// Tworzy nowe wymaganie typu BooleanPrecondition bedące kopią innego wymagania.
        /// </summary>
        /// <param name="precondition">Wymaganie do skopiowania.</param>
        public BooleanPrecondition(BooleanPrecondition precondition) : base(precondition)
        {
        }

        /// <summary>
        /// Łączy wymaganie z innym wymaganiem. Oba wymagania muszą być typu BooleanPrecondition i mieć tę samą wartość docelową.
        /// </summary>
        /// <param name="precondition">Wymaganie do połączenia.</param>
        /// <returns>Nowe wymaganie o wartości docelowej równej wartości docelowej obu wymagań lub null w przypadku, gdy wymagań nie można połączyć.</returns>
        protected internal override Precondition Combine(Precondition precondition)
        {
            if (!(precondition is BooleanPrecondition boolPre)
                || Variable.Id != boolPre.Variable.Id
                || Target != boolPre.Target)
            {
                return null;
            }
            bool state = IsFulfilled() || boolPre.IsFulfilled() ? Target : !Target;
            return new BooleanPrecondition(Target, state, this);
        }

        /// <summary>
        /// Porównuje wymaganie z innym wymaganiem. Oba wymagania muszą być typu BooleanPrecondition.
        /// </summary>
        /// <param name="precondition">Wymaganie do porównania.</param>
        /// <returns>ComparisonResult.Better, jeżeli obecne wymaganie jest w lepszym (spełnionym) stanie niż precondition; ComparisonResult.EqualWorse, jeżeli jest w takim samym bądź gorszym stanie niż precondition; ComparisonResult.NotComparable, jeżeli nie jest możliwe porównanie wymagań.</returns>
        protected internal override ComparisonResult IsBetterThan(Precondition precondition)
        {
            if (!(precondition is BooleanPrecondition boolPre)
                || Variable.Id != boolPre.Variable.Id
                || Target != boolPre.Target)
            {
                return ComparisonResult.NotComparable;
            }
            // we can only be better if we're fulfilled
            return IsFulfilled() ? ComparisonResult.Better : ComparisonResult.EqualWorse;
        }

        /// <summary>
        /// Sprawdza, czy wymaganie o danym stanie jest spełnione.
        /// Wymaganie jest spełnione, gdy jego stan jest równy wartości docelowej.
        /// </summary>
        /// <param name="state">Stan wymagania.</param>
        /// <param name="target">Wartość docelowa wymagania.</param>
        /// <returns>true, jeżeli wymaganie jest spełnione; false w przeciwnym wypadku.</returns>
        protected internal override bool IsFulfilled(bool state, bool target)
        {
            return state == target;
        }

        /// <summary>
        /// Sprawdza, czy wymaganie o danym stanie jest spełnione dla danej wartości powiązanej zmiennej.
        /// Wymaganie jest spełnione, gdy jego stan bądź wartość powiązanej zmiennej jest równa wartości docelowej.
        /// </summary>
        /// <param name="value">Wartość powiązanej zmiennej.</param>
        /// <param name="target">Stan wymagania.</param>
        /// <param name="state">Wartość docelowa wymagania.</param>
        /// <returns>true, jeżeli wymaganie jest spełnione; false w przeciwnym wypadku.</returns>
        protected internal override bool IsFulfilledInState(bool value, bool target, bool state)
        {
            return state == target || value == target;
        }

        /// <summary>
        /// Zwraca wartość stanu, z jaką wymaganie powinno rozpoczynać obliczenia. Wartość ta jest odwrotnością wartości docelowej.
        /// </summary>
        /// <returns>Początkowa wartość stanu wymagania.</returns>
        protected internal override bool GetDefault()
        {
            return !Target;
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
