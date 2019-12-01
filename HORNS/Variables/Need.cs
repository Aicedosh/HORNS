using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class Need<T> : Variable<T>, INeedInternal
    {
        private readonly Func<T, float> evaluation;

        private protected override T _Value { get => Variable.Value; set => Variable.Value = value; }
        internal Variable<T> Variable { get; private set; }
        public T Desired { get; private set; }

        internal override VariableSolver<T> GenericSolver => Variable.GenericSolver;

        private Need(Need<T> other) : base(other)
        {
            Variable = other.Variable;
            Desired = other.Desired;
        }

        public Need(Variable<T> variable, T desired, Func<T, float> evaluation)
        {
            Variable = variable;
            Desired = desired;
            this.evaluation = evaluation;
        }

        public override float Evaluate(T value)
        {
            return evaluation(value);
        }

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

        int IIdentifiable.Id => Variable.Id;
        IIdentifiable IIdentifiable.GetCopy()
        {
            return this; //TODO: Is this legal?
        }
    }
}
