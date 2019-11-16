using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class Variable
    {
        private static int MaxId = 0;
        internal int Id { get; private set; }

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
    }
}
