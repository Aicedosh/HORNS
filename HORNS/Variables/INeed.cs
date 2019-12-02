using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public interface INeed
    {
        // TODO: rename to Evaluate?
        float GetPriority();
        bool IsSatisfied();
    }
}
