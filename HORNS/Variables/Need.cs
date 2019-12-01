using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class Need<T> : Variable<T>, INeedInternal
    {
        private protected override T _Value { get => Variable.Value; set => Variable.Value = value; }
        internal Variable<T> Variable { get; private set; }
        public T Desired { get; private set; }

        internal override VariableSolver<T> GenericSolver => Variable.GenericSolver;

        public Need(Variable<T> variable, T desired)
        {
            Variable = variable;
            Desired = desired;
        }

        public abstract override float Evaluate(T value);

        public float GetPriority()
        {
            return Evaluate(Value);
        }

        internal float EvaluateFor(Variable variable)
        {
            return Evaluate((variable as Variable<T>).Value);
        }

        IEnumerable<Action> GetActionsTowards(Agent agent)
        {
            return GenericSolver.GetActionsTowards(Variable, Desired, agent);
        }

        public bool IsSatisfied()
        {
            return IsSatisfied(_Value);
        }

        protected virtual bool IsSatisfied(T value)
        {
            return value.Equals(Desired);
        }

        Variable GetVariable()
        {
            return Variable;
        }

        float INeedInternal.EvaluateFor(Variable variable) => EvaluateFor(variable);
        Variable INeedInternal.GetVariable() => GetVariable();
        IEnumerable<Action> INeedInternal.GetActionsTowards(Agent agent) => GetActionsTowards(agent);
    }
}
