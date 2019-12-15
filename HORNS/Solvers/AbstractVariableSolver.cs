using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Abstrakcyjna klasa bazowa dla solverów dla zmiennych typu T.
    /// </summary>
    /// <typeparam name="T">Typ danych przechowywanych w zmiennej związanej z solverem.</typeparam>
    public abstract class VariableSolver<T>
    {
        internal Variable<T> Variable { get; set; }
        internal abstract IEnumerable<Action> GetActionsSatisfying(Precondition<T> precondition, Agent agent);
        /// <summary>
        /// Wyznacza akcje modyfikujące daną zmienną w kierunku określonej wartości docelowej.
        /// </summary>
        /// <param name="variable">Zmienna.</param>
        /// <param name="desiredValue">Wartość docelowa.</param>
        /// <param name="agent">Agent, którego akcje będą rozważane.</param>
        /// <returns>Kolekcja akcji.</returns>
        internal abstract IEnumerable<Action> GetActionsTowards(Variable<T> variable, T desiredValue, Agent agent);
    }
}
