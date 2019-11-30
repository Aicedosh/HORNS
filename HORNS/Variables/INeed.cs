using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public interface INeed
    {
        float GetPriority(); //Maybe...?
        float EvaluateFor(Variable variable);

        IEnumerable<Action> GetActionsTowards();
        bool IsSatisfied();
        Variable GetVariable();
    }
}
