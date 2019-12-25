using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca rezultat związany ze zmienną typu \texttt{bool}, który powoduje przyjęcie przez zmienną określonej wartości.
    /// </summary>
    public class BooleanResult : ActionResult<bool, BooleanSolver>
    {
        /// <summary>
        /// Wartość, jaką ma przyjąć zmienna po wykonaniu rezultatu.
        /// </summary>
        public bool EndValue { get; }

        //TODO: [!] Replace with static factory methods?
        /// <summary>
        /// Tworzy nowy rezultat związany dla zmiennej typu \texttt{bool}, którego wynikiem jest zmiana wartości zmiennej na określoną wartość.
        /// </summary>
        /// <param name="endValue">Wartość końcowa rezultatu.</param>
        public BooleanResult(bool endValue)
        {
            EndValue = endValue;
        }

        /// <summary>
        /// Zwraca wartość końcową rezultatu dla danej wartości początkowej.
        /// Wartość końcowa jest równa \texttt{EndValue}.
        /// </summary>
        /// <param name="value">Wartość początkowa.</param>
        /// <returns>Wartość końcowa rezultatu.</returns>
        protected internal override bool GetResultValue(bool value)
        {
            return EndValue;
        }
    }
}
