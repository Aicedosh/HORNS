using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HORNS
{
    public class Variable : IIdentifiable
    {
        public static ReaderWriterLockSlim VariableLock = new ReaderWriterLockSlim();

        private static int MaxId = 0;
        internal int Id { get; private set; }

        int IIdentifiable.Id => Id;

        private ICollection<IVariableObserver> observers = new HashSet<IVariableObserver>();

        public void Observe(IVariableObserver observer)
        {
            observers.Add(observer);
        }

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

        internal virtual Variable GetCopy()
        {
            return new Variable(this);
        }

        IIdentifiable IIdentifiable.GetCopy()
        {
            return GetCopy();
        }

        #region Predefined variable types
        public static Variable<bool> CreateBoolean(bool value)
        {
            return new Variable<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(value);
        }

        //TODO: replace to template
        public static Variable<int> CreateInteger(int value)
        {
            return new Variable<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(value);
        }
        #endregion
    }
}
