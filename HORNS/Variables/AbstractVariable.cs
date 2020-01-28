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
        /// <param name="observer">Obserwator, który będzie otrzymywał informacje o zmianach danej zmiennej.</param>
        public void Observe(IVariableObserver observer)
        {
            observers.Add(observer);
        }

        /// <summary>
        /// Usuwa obserwatora z listy obserwatorów zmiennej.
        /// </summary>
        /// <param name="observer">Obserwator, który powinien przestać otrzymywać informacje o zmianach danej zmiennej.</param>
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
    }
}
