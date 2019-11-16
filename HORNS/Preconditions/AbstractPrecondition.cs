using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class Precondition
    {
        internal abstract Requirement GetRequirement(VariableSet variablePatch);
    }
}
