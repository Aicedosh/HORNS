using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class Precondition<T, ST> : Precondition<T> where ST : VariableSolver<T>
    {
        private ST solver;

        public Precondition(T value) : base(value)
        {

        }

        protected Precondition(T value, Precondition<T, ST> other) : this(value)
        {
            this.Variable = other.Variable;
            this.solver = other.solver;
        }

        internal void SetSolver(ST solver)
        {
            this.solver = solver;
        }

        public Precondition(Precondition<T, ST> precondition) : base(precondition)
        {
            solver = precondition.solver;
        }

        protected internal override IEnumerable<Action> GetActions()
        {
            return solver.GetActionsSatisfying(this);
        }
    }
}
