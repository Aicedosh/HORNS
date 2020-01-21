using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Interfejs dla obserwatorów zmiennych typu T.
    /// </summary>
    /// <typeparam name="T">Typ obserwowanej zmiennej.</typeparam>
    public interface IVariableObserver<T>
    {
        /// <summary>
        /// Metoda wywoływana w reakcji na zmianę obserwowanej zmiennej.
        /// </summary>
        /// <param name="oldValue">Poprzednia wartość zmiennej.</param>
        /// <param name="newValue">Wartość zmiennej po zmianie.</param>
        void ValueChanged(T oldValue, T newValue);
    }
}
