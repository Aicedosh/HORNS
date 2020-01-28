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
        internal abstract IEnumerable<Action> GetActionsTowards(Variable<T> variable, T desiredValue, Agent agent);
    }
}
