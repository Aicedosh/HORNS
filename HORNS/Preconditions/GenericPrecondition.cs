using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Abstrakcyjna klasa bazowa dla wymagań dla zmiennych typu T.
    /// </summary>
    /// <typeparam name="T">Typ danych przechowywanych w zmiennej związanej z wymaganiem.</typeparam>
    public abstract class Precondition<T> : Precondition
    {
        /// <summary>
        /// Zmienna związana z wymaganiem.
        /// </summary>
        protected internal Variable<T> Variable { get; internal set; }
        /// <summary>
        /// Wartość docelowa wymagania.
        /// </summary>
        public T Target { get; }
        // TODO: [?] something better than this that still allows derived classes to access their stuff
        private bool _initialized = false;
        private T _state;
        // current state
        internal T State
        {
            get
            {
                if (!_initialized)
                {
                    _initialized = true;
                    _state = GetDefault();
                }
                return _state;
            }
            set
            {
                _initialized = true;
                _state = value;
            }
        }

        /// <summary>
        /// Tworzy nowe wymaganie o określonej wartości docelowej.
        /// </summary>
        /// <param name="value">Wartość docelowa wymagania.</param>
        public Precondition(T target)
        {
            Target = target;
        }

        /// <summary>
        /// Tworzy nowe wymaganie bedące kopią innego wymagania.
        /// </summary>
        /// <param name="precondition">Wymaganie do skopiowania.</param>
        public Precondition(Precondition<T> precondition)
        {
            Target = precondition.Target;
            State = precondition.State;
            Variable = precondition.Variable;
        }
        
        protected Precondition(T value, T state, Precondition<T> precondition) : this(value)
        {
            Variable = precondition.Variable;
            State = state;
        }

        internal override Variable GetVariable()
        {
            return Variable;
        }

        /// <summary>
        /// Sprawdza, czy dana wartość spełnia wymaganie.
        /// </summary>
        /// <param name="value">Wartość do sprawdzenia.</param>
        /// <param name="target">Wartość docelowa wymagania.</param>
        /// <returns>\texttt{true}, jeżeli wartość spełnia wymaganie.</returns>
        protected internal abstract bool IsFulfilled(T value, T target);
        protected internal abstract bool IsFulfilledInState(T value, T target, T state);
        protected internal abstract T GetDefault();

        internal override bool IsFulfilled()
        {
            return IsFulfilled(State, Target);
        }

        internal override bool IsFulfilledByWorld()
        {
            return IsFulfilledInState(Variable.Value, Target, State);
        }

        internal override bool IsFulfilledBy(IdSet<Variable> variables)
        {
            Variable variable = Variable;
            if (variables != null)
            {
                variables.TryGet(ref variable);
            }
            return IsFulfilledInState((variable as Variable<T>).Value, Target, State);
        }

        internal override Precondition Apply(ActionResult actionResult)
        {
            var newPre = Clone() as Precondition<T>;
            newPre.State = (actionResult as ActionResult<T>).GetResultValue(newPre.State);
            return newPre;
        }

        /// <summary>
        /// Wyznacza akcje mogące spełnić wymaganie.
        /// </summary>
        /// <param name="agent">Agent, którego akcje będą rozważane.</param>
        /// <returns>Kolekcja akcji.</returns>
        protected internal override IEnumerable<Action> GetActions(Agent agent)
        {
            return Variable.GenericSolver.GetActionsSatisfying(this, agent);
        }
    }
}
