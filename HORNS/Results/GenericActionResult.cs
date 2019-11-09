using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class ActionResult<T> : ActionResult
    {
        protected Variable<T> Variable { get; private set; }
        internal override Variable AbstractVariable => Variable;

        private protected ActionResult(Variable<T> variable)
        {
            Variable = variable;
        }
    }
}
