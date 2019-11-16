using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class VariableSolver<T, RT, PT> : VariableSolver<T> where RT : ActionResult<T> where PT : Precondition<T>
    {
        protected internal abstract void Register(RT result);
        protected abstract IEnumerable<Action> GetActionsSatisfying(PT precondition);

        internal override IEnumerable<Action> GetActionsSatisfying(Precondition<T> precondition)
        {
            return GetActionsSatisfying(precondition as PT); //TODO: Don't really like the cast
        }
    }
}
