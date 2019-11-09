using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class VariableSolver<T, RT> : VariableSolver<T> where RT : ActionResult<T>
    {
        public void Register(RT result)
        {
            throw new NotImplementedException();
        }
    }
}
