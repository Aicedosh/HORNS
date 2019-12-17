using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca rezultat związany ze zmienną typu \texttt{int}, który powoduje zwiększenie lub zmniejszenie wartości zmiennej o stałą wartość.
    /// </summary>
    public class IntegerAddResult : ActionResult<int, IntegerSolver>
    {
        /// <summary>
        /// Składnik (dodatni lub ujemny) dodawany do wartości zmiennej w wyniku wykonania rezultatu.
        /// </summary>
        public int Term { get; }

        /// <summary>
        /// Tworzy nowy rezultat dla zmiennej typu \texttt{int}, którego wynikiem jest dodanie \texttt{term} do wartości zmiennej.
        /// </summary>
        /// <param name="term">Składnik (dodatni lub ujemny) dodawany do wartości zmiennej.</param>
        public IntegerAddResult(int term)
        {
            Term = term;
        }

        /// <summary>
        /// Zwraca wartość końcową rezultatu dla wartości początkowej reprezentowanej daną zmienną.
        /// Wartość końcowa jest równa sumie wartości początkowej oraz \texttt{Term}.
        /// </summary>
        /// <param name="variable">Zmienna o wartości początkowej.</param>
        /// <returns>Wartość końcowa rezultatu.</returns>
        protected internal override int GetResultValue(Variable<int> variable)
        {
            return variable.Value + Term;
        }
    }
}
