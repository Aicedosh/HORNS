using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class Precondition<T, ST> : Precondition<T> where ST : VariableSolver<T>
    {
        private readonly ST solver;

        public Precondition(Variable<T> variable, T value, ST solver) : base(variable, value)
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
