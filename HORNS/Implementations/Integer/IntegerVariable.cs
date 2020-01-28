using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca zmienne typu int.
    /// </summary>
    public class IntegerVariable : Variable<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>
    {
        /// <summary>
        /// Tworzy nową zmienną typu int o określonej wartości początkowej.
        /// </summary>
        /// <param name="value">Wartość początkowa zmiennej.</param>
        public IntegerVariable(int value = default) : base(value)
        {

        }
    }
}
