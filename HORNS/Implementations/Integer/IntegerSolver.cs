using System;
using System.Collections.Generic;
using System.Linq;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca solver dla zmiennych typu \texttt{int}, rezultatów reprezentujących zmianę wartości o określoną stałą i wymagań reprezentujących osiągnięcie wartości nie większej/nie mniejszej od określonej stałej.
    /// </summary>
    public class IntegerSolver : VariableSolver<int, IntegerAddResult, IntegerPrecondition>
    {
        List<IntegerAddResult> positiveResults = new List<IntegerAddResult>();
        List<IntegerAddResult> negativeResults = new List<IntegerAddResult>();

        /// <summary>
        /// Wyznacza akcje mogące spełnić dane wymaganie.
        /// Akcje mogące spełnić dane wymaganie to akcje zmniejszające wartość zmiennej dla kierunku AtMost lub zwiększające ją dla kierunku AtLeast.
        /// </summary>
        /// <param name="precondition">Wymaganie do spełnienia.</param>
        /// <returns>Kolekcja akcji.</returns>
        protected override IEnumerable<Action> GetActionsSatisfying(IntegerPrecondition precondition)
        {
            return (precondition.Direction == IntegerPrecondition.Condition.AtLeast ? positiveResults : negativeResults)
                .Select(res => res.Action);
        }

        /// <summary>
        /// Wyznacza akcje modyfikujące daną zmienną w kierunku określonej wartości docelowej.
        /// Jeżeli obecna wartość jest mniejsza od docelowej, będą to akcje zwiększające wartość; jeżeli jest większa, będą to akcje zmniejszające ją.
        /// </summary>
        /// <param name="variable">Zmienna.</param>
        /// <param name="desiredValue">Wartość docelowa.</param>
        /// <returns>Kolekcja akcji.</returns>
        protected override IEnumerable<Action> GetActionsTowards(Variable<int> variable, int desiredValue)
        {
            if (variable.Value == desiredValue)
            {
                return new List<Action>();
            }
            return (variable.Value < desiredValue ? positiveResults : negativeResults).Select(res => res.Action);
        }

        /// <summary>
        /// Dodaje rezultat do rezultatów rozważanych przez solver.
        /// </summary>
        /// <param name="result">Rezultat.</param>
        protected internal override void Register(IntegerAddResult result)
        {
            if (result.Term > 0)
            {
                positiveResults.Add(result);
            }
            else if (result.Term < 0)
            {
                negativeResults.Add(result);
            }
        }
    }
}
