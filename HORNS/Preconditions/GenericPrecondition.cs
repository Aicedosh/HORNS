using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class Precondition<T> : Precondition
    {
        protected internal Variable<T> Variable { get; internal set; }
        public T Value { get; }
        // current state
        internal T State { get; set; }

        public Precondition(T value)
        {
            Value = value;
            State = value;
        }

        public Precondition(Precondition<T> precondition)
        {
            Value = precondition.Value;
            State = precondition.State;
            Variable = precondition.Variable;
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

        internal override bool IsFulfilledBy(IdSet<Variable> variables)
        {
            Variable variable = Variable;
            if (variables != null)
            {
                variables.TryGet(ref variable);
            }
            return IsFulfilled((variable as Variable<T>).Value);
        }
    }
}
