using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Abstrakcyjna klasa bazowa dla rezultatów dla zmiennych typu T powiązanych z solverem typu ST.
    /// </summary>
    /// <typeparam name="T">Typ danych przechowywanych w zmiennej związanej z rezultatem.</typeparam>
    /// <typeparam name="ST">Typ solvera związanego z rezultatem.</typeparam>
    public abstract class ActionResult<T, ST> : ActionResult<T> where ST : VariableSolver<T>
    {
        void SetAction(Action action)
        {
            Action = action;
        }
    }
}
