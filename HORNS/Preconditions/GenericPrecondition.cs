using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class Precondition<T> : Precondition
    {
        // reference to global variable
        protected Variable<T> Variable { get; }
        // target value
        public T Value { get; }
        // current state
        internal T State { get; set; }

        public Precondition(Variable<T> variable, T value) : base(variable.Id)
        {
            Variable = variable;
            Value = value;
            State = value;
        }

        public Precondition(Precondition<T> precondition) : base(precondition.Variable.Id)
        {
            Variable = precondition.Variable;
            Value = precondition.Value;
            State = precondition.State;
        }

        internal override Variable GetVariable()
        {
            return Variable;
        }

        protected internal abstract bool IsFulfilled(T value);
        protected internal abstract bool IsZeroed(T value);

        internal override bool IsFulfilled()
        {
            return IsZeroed(State);
        }

        internal override bool IsFulfilledByWorld()
        {
            return IsFulfilled(Variable.Value);
        }
    }
}
