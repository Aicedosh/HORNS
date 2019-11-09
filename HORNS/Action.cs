using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HORNS
{
    public class Action
    {
        private ICollection<ActionResult> results = new List<ActionResult>();

        public void AddResult<T, RT, ST>(RT result, ST solver)
            where ST : VariableSolver<T, RT>
            where RT : ActionResult<T, ST>
        {
            solver.Register(result);
            results.Add(result);
        }

        internal IEnumerable<Variable> GetVariables()
        {
            List<Variable> variables = new List<Variable>();
            variables.AddRange(results.Select(r => r.AbstractVariable));
            //TODO: Add from preconditions
            return variables;
        }
    }
}
