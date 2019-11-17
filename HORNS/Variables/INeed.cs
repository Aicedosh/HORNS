using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public interface INeed
    {
        float GetPriority(); //Maybe...?

        IEnumerable<Action> GetActionsTowards();
    }
}
