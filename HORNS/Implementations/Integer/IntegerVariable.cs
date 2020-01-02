using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca zmienne typu bool.
    /// </summary>
    public class IntegerConsumeVariable : Variable<int, IntegerAddResult, IntegerConsumeSolver, IntegerConsumePrecondition>
    {
        /// <summary>
        /// Tworzy nową zmienną typu int o określonej wartości początkowej.
        /// </summary>
        /// <param name="value">Wartość początkowa zmiennej.</param>
        public IntegerConsumeVariable(int value = default) : base(value)
        {

        }
    }

    //public class IntegerSimpleVariable : Variable<int, IntegerAddResult, IntegerSimpleSolver, IntegerSimplePrecondition>
    //{
    //    // WorstBound here perhaps?
    //    public IntegerSimpleVariable(int value = default) : base(value)
    //    {

    //    }
    //}
}
