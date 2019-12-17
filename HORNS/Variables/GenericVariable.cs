using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    //TODO: Replace with interface?
    /// <summary>
    /// Abstrakcyjna klasa bazowa dla zmiennych przechowujących dane typu T.
    /// </summary>
    /// <typeparam name="T">Typ danych.</typeparam>
    public abstract class Variable<T> : Variable, IEvaluable<T>
    {
        internal abstract VariableSolver<T> GenericSolver { get; }
        private ICollection<IVariableObserver<T>> observers = new HashSet<IVariableObserver<T>>();

        private T _value;

        /// <summary>
        /// Tworzy nową zmienną o określonej wartości początkowej.
        /// </summary>
        /// <param name="value">Wartość początkowa zmiennej.</param>
        public Variable(T value = default)
        {
            _value = value;
        }

        /// <summary>
        /// Tworzy nową zmienną będącą kopią innej zmiennej.
        /// </summary>
        /// <param name="variable">Zmienna do skopiowania.</param>
        protected Variable(Variable<T> variable) : base(variable)
        {
            _value = variable._value;
        }

        private protected virtual T _Value
        {
            get
            {
                // TODO: add read lock here as well?
                return _value;
            }
            set
            {
                VariableLock.EnterWriteLock();
                _value = value;
                VariableLock.ExitWriteLock();
                Notify(value);
            }
        }

        /// <summary>
        /// Wartość zmiennej.
        /// </summary>
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

        /// <summary>
        /// Dodaje obserwatora do listy obserwatorów zmiennej.
        /// </summary>
        /// <param name="observer">Obserwator.</param>
        public void Observe(IVariableObserver<T> observer)
        {
            observers.Add(observer);
        }

        /// <summary>
        /// Usuwa obserwatora z listy obserwatorów zmiennej.
        /// </summary>
        /// <param name="observer">Obserwator.</param>
        public void Unobserve(IVariableObserver<T> observer)
        {
            observers.Remove(observer);
        }

        /// <summary>
        /// Oblicza ocenę zmiennej dla danej wartości.
        /// </summary>
        /// <param name="value">Wartość do oceny.</param>
        /// <returns>Ocena wartości.</returns>
        public virtual float Evaluate(T value)
        {
            return 0f;
        }

    }

    //TODO: [!] Move me to new file
    /// <summary>
    /// Abstrakcyjna klasa bazowa dla zmiennych typu T powiązanych z typami rezultatu, solvera i wymagania.
    /// </summary>
    /// <typeparam name="T">Typ danych przechowywanych przez zmienną.</typeparam>
    /// <typeparam name="RT">Typ rezultatu związany ze zmienną.</typeparam>
    /// <typeparam name="ST">Typ solvera związany ze zmienną.</typeparam>
    /// <typeparam name="PT">Typ wymagania związany ze zmienną.</typeparam>
    public class Variable<T, RT, ST, PT> : Variable<T>
            where ST : VariableSolver<T, RT, PT>, new()
            where RT : ActionResult<T, ST>
            where PT : Precondition<T, ST>
    {
        /// <summary>
        /// Tworzy nową zmienną o określonej wartości początkowej.
        /// </summary>
        /// <param name="value">Wartość początkowa zmiennej.</param>
        protected Variable(T value = default) : base(value)
        {
            Solver = new ST();
            Solver.Variable = this;
        }

        internal Variable(Variable<T, RT, ST, PT> variable) : base(variable)
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
    /// <summary>
    /// Klasa reprezentująca zmienne typu bool.
    /// </summary>
    public class BoolVariable : Variable<bool, BooleanResult, BooleanSolver, BooleanPrecondition>
    {
        /// <summary>
        /// Tworzy nową zmienną typu bool o określonej wartości początkowej.
        /// </summary>
        /// <param name="value">Wartość początkowa zmiennej.</param>
        public BoolVariable(bool value = default) : base(value)
        {
        }
    }

    /// <summary>
    /// Klasa reprezentująca zmienne typu bool.
    /// </summary>
    public class IntVariable : Variable<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>
    {
        /// <summary>
        /// Tworzy nową zmienną typu int o określonej wartości początkowej.
        /// </summary>
        /// <param name="value">Wartość początkowa zmiennej.</param>
        public IntVariable(int value = default) : base(value)
        {

        }
    }

}
