using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    //TODO: Replace with interface?
    public abstract class Variable<T> : Variable, IEvaluable<T>
    {
        internal abstract VariableSolver<T> GenericSolver { get; }
        private ICollection<IVariableObserver<T>> observers = new HashSet<IVariableObserver<T>>();

        private T _value;

        public Variable(T value = default)
        {
            _value = value;
        }

        protected Variable(Variable<T> variable) : base(variable)
        {
            _value = variable._value;
        }

        private protected virtual T _Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                Notify(value);
            }
        }

        public T Value
        {
            get => _Value;
            set => _Value = value;
        }

        private void Notify(T value)
        {
            foreach (IVariableObserver<T> observer in observers)
            {
                observer.ValueChanged(value);
            }
            base.Notify();
        }

        public void Observe(IVariableObserver<T> observer)
        {
            observers.Add(observer);
        }

        public void Unobserve(IVariableObserver<T> observer)
        {
            observers.Remove(observer);
        }

        public virtual float Evaluate(T value)
        {
            return 0f;
        }

    }

    //TODO: [!] Move me to new file
    public class Variable<T, RT, ST, PT> : Variable<T>
            where ST : VariableSolver<T, RT, PT>, new()
            where RT : ActionResult<T, ST>
            where PT : Precondition<T, ST>
    {
        public Variable(T value = default) : base(value)
        {
            Solver = new ST();
            Solver.Variable = this;
        }

        public Variable(Variable<T, RT, ST, PT> variable) : base(variable)
        {
            Solver = variable.Solver;
        }

        internal override Variable GetCopy()
        {
            return new Variable<T, RT, ST, PT>(this);
        }

        internal ST Solver { get; }

        internal override VariableSolver<T> GenericSolver => Solver;
    }

    //TODO: [!] Not the best solution
    public class BoolVariable : Variable<bool, BooleanResult, BooleanSolver, BooleanPrecondition>
    {
        public BoolVariable(bool value = default) : base(value)
        {
        }
    }

    public class IntVariable : Variable<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>
    {
        public IntVariable(int value = default) : base(value)
        {

        }
    }

}
