using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class Need<T> : Variable<T>
    {
        private protected override T _Value { get => variable.Value; set => variable.Value = value; }
        private Variable<T> variable;
        public T Desired { get; private set; }

        public Need(T desired)
        {
            Desired = desired;
        }

        public abstract override double Evaluate(T value);
    }
}
