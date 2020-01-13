using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    internal interface INeedInternal : INeed, IIdentifiable
    {
        float EvaluateFor(Variable variable);
        float EvaluateFor(IdSet<Variable> variables);
        bool IsSatisfied(IdSet<Variable> variables);

        IEnumerable<Action> GetActionsTowards(Agent agent);
        Variable GetVariable();
    }
}
