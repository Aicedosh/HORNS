using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class VariableSolver<T, RT> : VariableSolver<T> where RT : ActionResult<T>
    {
        protected internal abstract void Register(RT result);
    }
}
