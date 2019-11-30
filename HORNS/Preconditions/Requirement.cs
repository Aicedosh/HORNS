//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace HORNS.Deprecated
//{
//    public abstract class Requirement : IIdentifiable
//    {
//        internal int Id { get; private protected set; }
//        protected internal abstract bool IsEqualOrWorse(Requirement other);
//        protected internal abstract IEnumerable<Action> GetActions();
//        internal bool Fulfilled { get; private protected set; } = false;    // TODO: property + methods = dis ugly

//        int IIdentifiable.Id => Id;

//        internal abstract bool IsFulfilled();
//        internal abstract bool IsFulfilled(IdSet<Variable> variables);
//        internal abstract Requirement Clone();

//        IIdentifiable IIdentifiable.GetCopy()
//        {
//            return Clone();
//        }

//        internal abstract Requirement Subtract(ActionResult actionResult);
//        internal abstract Requirement Combine(Requirement requirement);
//    }

//    // ----------

//    internal class PreconditionRequirement : Requirement
//    {
//        internal readonly Precondition<T> precondition;

//        public PreconditionRequirement(Precondition<T> precondition)
//        {
//            this.precondition = precondition;
//            Id = precondition.Variable.Id;
//        }

//        internal PreconditionRequirement(PreconditionRequirement requirement) :
//            this(requirement.precondition)
//        {
//        }

//        protected internal override IEnumerable<Action> GetActions()
//        {
//            return precondition.GetActions();
//        }

//        // also: maybe it doesn't need to be cached, we should just move it to another set with fulfilled reqs?
//        // although it prolly needs to be cached, we can't change unfulfilled ReqSet while iterating over it...
//        internal override bool IsFulfilled(/*VariableSet variablePatch*/)
//        {
//            //Variable var = variable;
//            //variablePatch.TryGet(ref var);
//            //Variable<T> v = var as Variable<T>;
//            //Fulfilled = precondition.IsFulfilled(v.Value);
//            Fulfilled = precondition.IsReqZeroed(precondition.Value);
//            return Fulfilled;
//        }

//        // TODO: [A] look at this please, [M] can probably do it but tell her if she's rambling or not
//        // this is supposed to be a method to check whether given VariableSet fulfills it
//        // not as a patch, but as a whole set - basically we'll be passing agent's VariableSet
//        // didn't cache the result - in my mind Fulfilled means that stored value already fulfills it
//        internal override bool IsFulfilled(IdSet<Variable> variables)
//        {
//            Variable var = precondition.Variable;
//            if (variables.TryGet(ref var))
//            {
//                return precondition.IsFulfilled((var as Variable<T>).Value);
//            }
//            return false;
//        }

//        protected internal override bool IsEqualOrWorse(Requirement other)
//        {
//            // TODO: this is temporary, change this!!!!!!!!!!!!!!!!!!!!!!!!!
//            var preOther = other as PreconditionRequirement;
//            if (preOther == null)
//            {
//                return false;
//            }
//            return precondition.IsEqualOrWorse(preOther.precondition);
//        }

//        internal override Requirement Clone()
//        {
//            return new PreconditionRequirement(this);
//        }

//        internal override Requirement Subtract(ActionResult actionResult)
//        {
//            var res = precondition.Subtract(this, actionResult as ActionResult<T>);
//            res.IsFulfilled();
//            return res;
//        }

//        internal override Requirement Combine(Requirement requirement)
//        {
//            if (!(requirement is PreconditionRequirement preReq))
//            {
//                return null;
//            }
//            var pre = precondition.Combine(preReq.precondition);
//            return pre?.GetRequirement();
//        }
//    }
//}
