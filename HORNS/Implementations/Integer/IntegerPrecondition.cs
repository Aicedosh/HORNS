using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca wymaganie związane ze zmienną typu int, które jest spełnione dla wartości zmiennej nie mniejszych od określonej stałej.
    /// Wartość docelowa powinna być dodatnia.
    /// </summary>
    public class IntegerPrecondition : Precondition<int>
    {
        /// <summary>
        /// Zwraca wartość informującą o tym, czy wartość zmiennej wymagana przez wymaganie zostanie zużyta przez rezultat typu IntegerAddResult.
        /// </summary>
        public bool Consumed { get; }

        /// <summary>
        /// Tworzy nowe wymaganie dla zmiennej typu int o określonej wartości wymaganej.
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
        /// Tworzy nowe wymaganie typu IntegerPrecondition bedące kopią innego wymagania.
        /// </summary>
        /// <param name="precondition">Wymaganie do skopiowania.</param>
        public IntegerPrecondition(IntegerPrecondition precondition) : base(precondition)
        {
            Consumed = precondition.Consumed;
        }
        
        /// <summary>
        /// Łączy wymaganie z innym wymaganiem. Oba wymagania muszą być typu IntegerPrecondition.
        /// </summary>
        /// <param name="precondition">Wymaganie do połączenia.</param>
        /// <returns>Nowe wymaganie będące wynikiem połączenia wymagań lub null w przypadku, gdy wymagań nie można połączyć.</returns>
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
        /// Porównuje wymaganie z innym wymaganiem. Oba wymagania muszą być typu IntegerPrecondition i mieć tę samą wartość docelową.
        /// </summary>
        /// <param name="precondition">Wymaganie do porównania.</param>
        /// <returns>ComparisonResult.Better, jeżeli obecne wymaganie jest w lepszym (bliższym wartości wymaganej) stanie niż precondition; ComparisonResult.EqualWorse, jeżeli jest w takim samym bądź gorszym stanie niż precondition; ComparisonResult.NotComparable, jeżeli nie jest możliwe porównanie wymagań.</returns>
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
        /// Sprawdza, czy wymaganie o danym stanie jest spełnione.
        /// Wymaganie jest spełnione, gdy jego stan jest nie mniejszy od wartości wymaganej.
        /// </summary>
        /// <param name="state">Stan wymagania.</param>
        /// <param name="target">Wartość docelowa wymagania.</param>
        /// <returns>true, jeżeli wymaganie jest spełnione; false w przeciwnym wypadku.</returns>
        protected internal override bool IsFulfilled(int state, int target)
        {
            return state >= target;
        }

        /// <summary>
        /// Sprawdza, czy wymaganie o danym stanie jest spełnione dla danej wartości powiązanej zmiennej. 
        /// Wymaganie jest spełnione, gdy suma wartości jego stanu i powiązanej zmiennej jest nie mniejsza od wartości wymaganej.
        /// </summary>
        /// <param name="value">Wartość powiązanej zmiennej.</param>
        /// <param name="target">Stan wymagania.</param>
        /// <param name="state">Wartość docelowa wymagania.</param>
        /// <returns>true, jeżeli wymaganie jest spełnione; false w przeciwnym wypadku.</returns>
        protected internal override bool IsFulfilledInState(int value, int target, int state)
        {
            return value + state >= target;
        }

        /// <summary>
        /// Zwraca wartość stanu, z jaką wymaganie powinno rozpoczynać obliczenia. Wartość ta wynosi 0.
        /// </summary>
        /// <returns>Początkowa wartość stanu wymagania.</returns>
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
