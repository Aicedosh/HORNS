using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    internal interface INeedInternal : INeed
    {
        float EvaluateFor(Variable variable);

        IEnumerable<Action> GetActionsTowards(Agent agent);
        Variable GetVariable();
    }
}
