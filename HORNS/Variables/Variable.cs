using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class Variable
    {
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

        }
    }
}
