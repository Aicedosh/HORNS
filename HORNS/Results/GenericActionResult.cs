using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class ActionResult<T> : ActionResult
    {
        private Variable<T> variable;

        internal override Variable GetVariable()
        {
            return variable;
        }
    }
}
