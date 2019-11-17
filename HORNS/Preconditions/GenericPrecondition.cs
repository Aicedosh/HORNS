using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class Precondition<T> : Precondition
    {
        protected Variable<T> Variable { get; private set; }

        public Precondition(Variable<T> variable)
        {
            Variable = variable;
        }

        protected class PreconditionRequirement : Requirement
        {
            private readonly Precondition<T> precondition;
            private readonly Variable<T> variable;

            public PreconditionRequirement(Precondition<T> precondition, Variable<T> variable)
            {
                this.precondition = precondition;
                this.variable = variable;
            }

            protected internal override IEnumerable<Action> GetActions()
            {
                return precondition.GetActions(variable);
            }

            internal override bool IsFulfilled(VariableSet variablePatch)
            {
                Variable var = variable;
                variablePatch.TryGet(ref var);
                Variable<T> v = var as Variable<T>;
                return precondition.IsFulfilled(v.Value);
            }

            protected internal override bool IsEqual(Requirement other)
            {
                throw new NotImplementedException(); //TODO: Something like this will be required to avoid going in loop
            }
        }

        protected abstract IEnumerable<Action> GetActions(Variable<T> variable);
        protected internal abstract bool IsFulfilled(T value);

        internal override Requirement GetRequirement(VariableSet variablePatch)
        {
            Variable var = Variable;
            variablePatch.TryGet(ref var);
            return new PreconditionRequirement(this, var as Variable<T>); //TODO: cast?
        }
    }
}
