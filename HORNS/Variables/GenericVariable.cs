using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class Variable<T> : Variable
    {
        private ICollection<IVariableObserver<T>> observers = new HashSet<IVariableObserver<T>>();

        private T _value;
        private protected virtual T _Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                Notify(value);
            }
        }

        public T Value
        {
            get => _Value;
            set => _Value = value;
        }

        private void Notify(T value)
        {
            foreach (IVariableObserver<T> observer in observers)
            {
                observer.ValueChanged(value);
            }
            base.Notify();
        }

        public void Observe(IVariableObserver<T> observer)
        {
            observers.Add(observer);
        }

        public void Unobserve(IVariableObserver<T> observer)
        {
            observers.Remove(observer);
        }

        public virtual float Evaluate(T value)
        {
            return 0f;
        }

        internal override Variable GetCopy()
        {
            var newVar = base.GetCopy() as Variable<T>;
            newVar._value = _value;
            return newVar;
        }
    }
}
