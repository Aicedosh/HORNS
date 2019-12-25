using System;
using System.Collections.Generic;
using System.Linq;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca solver dla zmiennych typu bool, rezultatów reprezentujących ustawienie określonej wartości i wymagań reprezentujących posiadanie określonej wartości.
    /// </summary>
    public class BooleanSolver : VariableSolver<bool, BooleanResult, BooleanPrecondition>
    {
        List<BooleanResult> trueResults = new List<BooleanResult>();
        List<BooleanResult> falseResults = new List<BooleanResult>();
        
        /// <summary>
        /// Wyznacza akcje mogące spełnić dane wymaganie.
        /// Akcje mogące spełnić dane wymaganie to akcje ustawiające odpowiednią wartość końcową.
        /// </summary>
        /// <param name="precondition">Wymaganie do spełnienia.</param>
        /// <returns>Kolekcja akcji.</returns>
        protected override IEnumerable<Action> GetActionsSatisfying(BooleanPrecondition precondition)
        {
            return GetActions(precondition.Target);
        }

        /// <summary>
        /// Wyznacza akcje modyfikujące daną zmienną w kierunku określonej wartości docelowej.
        /// Będą to akcje ustawiające wartość zmiennej na podaną wartość docelową.
        /// </summary>
        /// <param name="variable">Zmienna.</param>
        /// <param name="desiredValue">Wartość docelowa.</param>
        /// <returns>Kolekcja akcji.</returns>
        protected override IEnumerable<Action> GetActionsTowards(Variable<bool> variable, bool desiredValue)
        {
            return GetActions(desiredValue);
        }

        private IEnumerable<Action> GetActions(bool value)
        {
            return (value ? trueResults : falseResults).Select(res => res.Action);
        }

        /// <summary>
        /// Dodaje rezultat do rezultatów rozważanych przez solver.
        /// </summary>
        /// <param name="result">Rezultat.</param>
        protected internal override void Register(BooleanResult result)
        {
            var destList = result.EndValue ? trueResults : falseResults;
            destList.Add(result);
        }
    }
}
