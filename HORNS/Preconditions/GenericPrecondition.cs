using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class Precondition<T> : Precondition
    {
        internal abstract class PreconditionRequirement : ActionRequirement
        {

        }
    }
}
