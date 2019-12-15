using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HORNS
{
    /// <summary>
    /// Klasa bazowa dla wszystkich zmiennych.
    /// </summary>
    public abstract class Variable : IIdentifiable
    {
        internal static ReaderWriterLockSlim VariableLock = new ReaderWriterLockSlim();

        private static int MaxId = 0;
        internal int Id { get; private set; }

        int IIdentifiable.Id => Id;

        private ICollection<IVariableObserver> observers = new HashSet<IVariableObserver>();

        /// <summary>
        /// Dodaje obserwatora do listy obserwatorów zmiennej.
        /// </summary>
        /// <param name="observer">Obserwator.</param>
        public void Observe(IVariableObserver observer)
        {
            observers.Add(observer);
        }

        /// <summary>
        /// Usuwa obserwatora z listy obserwatorów zmiennej.
        /// </summary>
        /// <param name="observer">Obserwator.</param>
        public void Unobserve(IVariableObserver observer)
        {
            observers.Remove(observer);
        }

        private protected void Notify()
        {
            foreach(IVariableObserver observer in observers)
            {
                observer.ValueChanged();
            }
        }

        private protected Variable()
        {
            Id = MaxId++;
        }

        private protected Variable(Variable other)
        {
            Id = other.Id;
        }

        internal abstract Variable GetCopy();

        IIdentifiable IIdentifiable.GetCopy()
        {
            return GetCopy();
        }

        #region Predefined variable types
        /// <summary>
        /// Tworzy zmienną przechowującą wartości typu bool.
        /// </summary>
        /// <param name="value">Wartość początkowa zmiennej.</param>
        /// <returns></returns>
        public static Variable<bool> CreateBoolean(bool value)
        {
            return new Variable<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(value);
        }

        //TODO: replace to template
        /// <summary>
        /// Tworzy zmienną przechowującą wartości typu int.
        /// </summary>
        /// <param name="value">Wartość początkowa zmiennej.</param>
        /// <returns></returns>
        public static Variable<int> CreateInteger(int value)
        {
            return new Variable<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(value);
        }
        #endregion
    }
}
