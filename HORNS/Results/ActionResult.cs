using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class ActionResult<T, ST> : ActionResult<T> where ST : VariableSolver<T>
    {
        void SetAction(Action action)
        {
            Action = action;
        }
    }
}
