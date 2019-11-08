using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class VariableChangeObserver<T> : IVariableObserver<T>
    {
        private bool hasPreviousValue = false;
        private T previousValue;
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
            foreach (IVariableObserver observer in observers)
            {
                observer.ValueChanged();
            }
        }

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

        protected abstract bool NotifyValueChanged(T previousValue, T newValue);
    }
}
