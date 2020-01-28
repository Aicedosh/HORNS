using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Abstrakcyjna klasa bazowa dla solverów dla zmiennych typu T powiązanych z rezultatem typu RT i wymaganiem typu PT.
    /// </summary>
    /// <typeparam name="T">Typ danych przechowywanych w zmiennej związanej z solverem.</typeparam>
    /// <typeparam name="RT">Typ rezultatu związany z solverem.</typeparam>
    /// <typeparam name="PT">Typ wymagania związany z solverem.</typeparam>
    public abstract class VariableSolver<T, RT, PT> : VariableSolver<T> where RT : ActionResult<T> where PT : Precondition<T>
    {
        /// <summary>
        /// Dodaje rezultat do rezultatów rozważanych przez solver.
        /// </summary>
        /// <param name="result">Rezultat.</param>
        protected internal abstract void Register(RT result);
        /// <summary>
        /// Wyznacza akcje modyfikujące daną zmienną w kierunku określonej wartości docelowej.
        /// </summary>
        /// <param name="variable">Zmienna.</param>
        /// <param name="desiredValue">Wartość docelowa.</param>
        /// <returns>Kolekcja akcji.</returns>
        protected abstract IEnumerable<Action> GetActionsTowards(Variable<T> variable, T desiredValue);
        /// <summary>
        /// Wyznacza akcje mogące spełnić dane wymaganie.
        /// </summary>
        /// <param name="precondition">Wymaganie do spełnienia.</param>
        /// <returns>Kolekcja akcji.</returns>
        protected abstract IEnumerable<Action> GetActionsSatisfying(PT precondition);

        internal override IEnumerable<Action> GetActionsTowards(Variable<T> variable, T desiredValue, Agent agent)
        {
            return GetActionsTowards(variable, desiredValue).Where(a => agent.PossibleActions.Contains(a));
        }

        internal override IEnumerable<Action> GetActionsSatisfying(Precondition<T> precondition, Agent agent)
        {
            return GetActionsSatisfying(precondition as PT).Where(a=>agent.PossibleActions.Contains(a));
        }
    }
}
