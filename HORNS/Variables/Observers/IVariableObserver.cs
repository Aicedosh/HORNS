using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Interfejs dla obserwatorów zmiennych niezależnych od typu.
    /// </summary>
    public interface IVariableObserver
    {
        /// <summary>
        /// Metoda wywoływana w reakcji na zmianę obserwowanej zmiennej.
        /// </summary>
        void ValueChanged();
    }
}
