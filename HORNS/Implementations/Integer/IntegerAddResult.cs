using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class IntegerAddResult : ActionResult<int, IntegerSolver>
    {
        public int Term { get; }

        public IntegerAddResult(int term)
        {
            Term = term;
        }

        protected internal override int GetResultValue(Variable<int> variable)
        {
            return variable.Value + Term;
        }

        // TODO: is this necessary to override? maybe it's enough to Apply always using GetResultValue?
        internal override void Apply(IdSet<Variable> variables)
        {
            if (!variables.Contains(Variable.Id))
            {
                variables.Add(Variable.GetCopy());
            }
            var variable = variables[Variable.Id] as Variable<int>;
            variable.Value = GetResultValue(variable);
        }
    }
}
