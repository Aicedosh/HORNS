using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Abstrakcyjna klasa bazowa dla obserwatorów zmian zmiennych typu T.
    /// </summary>
    /// <typeparam name="T">Typ obserwowanej zmiennej.</typeparam>
    public abstract class VariableChangeObserver<T> : IVariableObserver<T>
    {
        private bool hasPreviousValue = false;
        private T previousValue;
        private ICollection<IVariableObserver> observers = new HashSet<IVariableObserver>();

        /// <summary>
        /// Dodaje obserwatora do listy do wywołania w przypadku konieczności notyfikacji.
        /// </summary>
        /// <param name="observer">Obserwator.</param>
        public void Observe(IVariableObserver observer)
        {
            observers.Add(observer);
        }

        /// <summary>
        /// Usuwa obserwatora z listy do wywołania.
        /// </summary>
        /// <param name="observer">Obserwator.</param>
        public void Unobserve(IVariableObserver observer)
        {
            observers.Remove(observer);
        }

        private protected void Notify()
        {
            foreach (IVariableObserver observer in observers)
            {
                observer.ValueChanged();
            }
        }

        /// <summary>
        /// Przekazuje notyfikację do obserwatorów, jeżeli notyfikacja wymaga obsługi.
        /// </summary>
        /// <param name="value">Wartość zmiennej po zmianie.</param>
        public void ValueChanged(T value)
        {
            if(!hasPreviousValue)
            {
                previousValue = value;
                return;
            }
            if(NotifyValueChanged(previousValue, value))
            {
                Notify();
                previousValue = value;
            }
        }

        /// <summary>
        /// Sprawdza, czy notyfikacja o zmianie zmiennej wymaga obsługi.
        /// </summary>
        /// <param name="previousValue">Poprzednia wartość zmiennej.</param>
        /// <param name="newValue">Nowa wartość zmiennej.</param>
        /// <returns>\texttt{true}, jeżeli notyfikacja wymaga obsługi.</returns>
        protected abstract bool NotifyValueChanged(T previousValue, T newValue);
    }
}
