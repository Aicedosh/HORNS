using System.Collections.Generic;

namespace HORNS
{
    public abstract class Precondition : IIdentifiable
    {
        internal int Id => GetVariable().Id;
        int IIdentifiable.Id => Id;

        protected internal abstract Precondition Clone();
        IIdentifiable IIdentifiable.GetCopy()
        {
            return Clone();
        }

        //private protected bool Fulfilled { get; set; } = false;
        internal abstract bool IsFulfilled();
        //internal abstract bool IsFulfilled(IdSet<Variable> variables);
        internal abstract bool IsFulfilledByWorld();

        protected internal abstract bool IsEqualOrWorse(Precondition other);
        protected internal abstract IEnumerable<Action> GetActions(Agent agent);
        protected internal abstract Precondition Subtract(ActionResult actionResult);
        protected internal abstract Precondition Combine(Precondition precondition);

        internal abstract Variable GetVariable();
    }
}
