using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class Requirement : IIdentifiable
    {
        internal int Id { get; private protected set; }
        protected internal abstract bool IsEqual(Requirement other);
        protected internal abstract IEnumerable<Action> GetActions();
        internal bool Fulfilled { get; private protected set; } = false;    // TODO: property + methods = dis ugly

        int IIdentifiable.Id => Id;

        internal abstract bool IsFulfilled();
        internal abstract bool IsFulfilled(IdSet<Variable> variables);
        internal abstract Requirement Clone();

        IIdentifiable IIdentifiable.GetCopy()
        {
            return Clone();
        }

        internal abstract Requirement Subtract(ActionResult actionResult);
    }
}
