using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class Need<T> : Variable<T>, INeed
    {
        private readonly VariableSolver<T> solver;
        private protected override T _Value { get => Variable.Value; set => Variable.Value = value; }
        internal Variable<T> Variable { get; private set; }
        public T Desired { get; private set; }

        public Need(Variable<T> variable, T desired, VariableSolver<T> solver)
        {
            Variable = variable;
            Desired = desired;
            this.solver = solver;
        }

        public abstract override float Evaluate(T value);

        public float GetPriority()
        {
            return Evaluate(Value);
        }

        public IEnumerable<Action> GetActionsTowards()
        {
            return solver.GetActionsTowards(Variable, Desired);
        }

        public bool IsSatisfied()
        {
            return _Value.Equals(Desired);
        }
    }
}
