using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class Precondition<T, ST> : Precondition<T> where ST : VariableSolver<T>
    {
        private readonly ST solver;

        public Precondition(Variable<T> variable, ST solver) : base(variable)
        {
            this.solver = solver;
        }

        protected override IEnumerable<Action> GetActions(Variable<T> variable)
        {
            return solver.GetActionsSatisfying(this);
        }
    }
}
