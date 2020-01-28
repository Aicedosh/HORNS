using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca zmienne typu bool.
    /// </summary>
    public class BooleanVariable : Variable<bool, BooleanResult, BooleanSolver, BooleanPrecondition>
    {
        /// <summary>
        /// Tworzy nową zmienną typu bool o określonej wartości początkowej.
        /// </summary>
        /// <param name="value">Wartość początkowa zmiennej.</param>
        public BooleanVariable(bool value = default) : base(value)
        {
        }
    }
}
