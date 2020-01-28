using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Abstrakcyjna klasa bazowa dla zmiennych przechowujących dane typu T.
    /// </summary>
    /// <typeparam name="T">Typ danych.</typeparam>
    public abstract class Variable<T> : Variable, IEvaluable<T>
    {
        internal abstract VariableSolver<T> GenericSolver { get; }
        private ICollection<IVariableObserver<T>> observers = new HashSet<IVariableObserver<T>>();

        private T _value;

        /// <summary>
        /// Tworzy nową zmienną o określonej wartości początkowej.
        /// </summary>
        /// <param name="value">Wartość początkowa zmiennej.</param>
        public Variable(T value = default)
        {
            _value = value;
        }

        /// <summary>
        /// Tworzy nową zmienną będącą kopią innej zmiennej.
        /// </summary>
        /// <param name="variable">Zmienna do skopiowania.</param>
        protected Variable(Variable<T> variable) : base(variable)
        {
            _value = variable._value;
        }

        private protected virtual T _Value
        {
            get
            {
                return _value;
            }
            set
            {
                T old = _value;
                VariableLock.EnterWriteLock();
                _value = value;
                VariableLock.ExitWriteLock();
                Notify(old, value);
            }
        }

        /// <summary>
        /// Obecna wartość zmiennej.
        /// </summary>
        public T Value
        {
            get => _Value;
            set => _Value = value;
        }

        private void Notify(T oldValue, T newValue)
        {
            foreach (IVariableObserver<T> observer in observers)
            {
                observer.ValueChanged(oldValue, newValue);
            }
            base.Notify();
        }

        /// <summary>
        /// Dodaje obserwatora do listy obserwatorów zmiennej.
        /// </summary>
        /// <param name="observer">Obserwator, który będzie otrzymywał informacje o zmianach danej zmiennej.</param>
        public void Observe(IVariableObserver<T> observer)
        {
            observers.Add(observer);
        }

        /// <summary>
        /// Usuwa obserwatora z listy obserwatorów zmiennej.
        /// </summary>
        /// <param name="observer">Obserwator, który powinien przestać otrzymywać informacje o zmianach danej zmiennej.</param>
        public void Unobserve(IVariableObserver<T> observer)
        {
            observers.Remove(observer);
        }

        /// <summary>
        /// Oblicza ocenę zmiennej dla danej wartości.
        /// </summary>
        /// <param name="value">Wartość, dla której należy wyznaczyć ocenę.</param>
        /// <returns>Ocena wartości, domyślnie zawsze 0.</returns>
        public virtual float Evaluate(T value)
        {
            return 0f;
        }

    }
}
