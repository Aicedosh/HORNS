using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class Need<T> : INeedInternal, IIdentifiable, IEvaluable<T>
    {
        private readonly Func<T, float> evaluation;

        public T Value { get => Variable.Value; }
        internal Variable<T> Variable { get; private set; }
        public T Desired { get; private set; }

        internal VariableSolver<T> GenericSolver => Variable.GenericSolver;

        private Need(Need<T> other)
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

        public float Evaluate(T value)
        {
            return evaluation(value);
        }

        public float Evaluate()
        {
            return Evaluate(Value);
        }

        internal float EvaluateFor(Variable variable)
        {
            return Evaluate((variable as Variable<T>).Value);
        }

        internal float EvaluateFor(IdSet<Variable> variables)
        {
            Variable variable = Variable;
            if (variables != null)
            {
                variables.TryGet(ref variable);
            }
            return EvaluateFor(variable);
        }

        IEnumerable<Action> GetActionsTowards(Agent agent)
        {
            return GenericSolver.GetActionsTowards(Variable, Desired, agent);
        }

        public bool IsSatisfied()
        {
            return IsSatisfied(Value);
        }

        internal bool IsSatisfied(IdSet<Variable> variables)
        {
            Variable variable = Variable;
            if (variables != null)
            {
                variables.TryGet(ref variable);
            }
            return IsSatisfied((variable as Variable<T>).Value);
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
        float INeedInternal.EvaluateFor(IdSet<Variable> variables) => EvaluateFor(variables);
        bool INeedInternal.IsSatisfied(IdSet<Variable> variables) => IsSatisfied(variables);
        Variable INeedInternal.GetVariable() => GetVariable();
        IEnumerable<Action> INeedInternal.GetActionsTowards(Agent agent) => GetActionsTowards(agent);

        int IIdentifiable.Id => Variable.Id;
        IIdentifiable IIdentifiable.GetCopy()
        {
            throw new NotImplementedException();
        }
    }
}
