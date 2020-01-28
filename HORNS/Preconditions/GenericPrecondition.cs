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

        private bool _initialized = false;
        private T _state;
        /// <summary>
        /// Aktualny stan wymagania.
        /// </summary>
        protected internal T State
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
        /// <param name="target">Wartość docelowa wymagania.</param>
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
        
        /// <summary>
        /// Tworzy nowe wymaganie na podstawie innego wymagania, o podanej wartości docelowej i stanie.
        /// </summary>
        /// <param name="target">Wartość docelowa nowego wymagania.</param>
        /// <param name="state">Początkowy stan nowego wymagania.</param>
        /// <param name="precondition">Wymaganie, na podstawie którego tworzone jest nowe wymaganie.</param>
        protected Precondition(T target, T state, Precondition<T> precondition) : this(target)
        {
            Variable = precondition.Variable;
            State = state;
        }

        internal override Variable GetVariable()
        {
            return Variable;
        }

        /// <summary>
        /// Sprawdza, czy wymaganie o danym stanie jest spełnione.
        /// </summary>
        /// <param name="state">Stan wymagania.</param>
        /// <param name="target">Wartość docelowa wymagania.</param>
        /// <returns>true, jeżeli wymaganie jest spełnione; false w przeciwnym wypadku.</returns>
        protected internal abstract bool IsFulfilled(T state, T target);

        /// <summary>
        /// Sprawdza, czy wymaganie o danym stanie jest spełnione dla danej wartości powiązanej zmiennej. 
        /// </summary>
        /// <param name="value">Wartość powiązanej zmiennej.</param>
        /// <param name="target">Stan wymagania.</param>
        /// <param name="state">Wartość docelowa wymagania.</param>
        /// <returns>true, jeżeli wymaganie jest spełnione; false w przeciwnym wypadku.</returns>
        protected internal abstract bool IsFulfilledInState(T value, T target, T state);

        /// <summary>
        /// Zwraca wartość stanu, z jaką wymaganie powinno rozpoczynać obliczenia.
        /// </summary>
        /// <returns>Początkowa wartość stanu wymagania.</returns>
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
        internal override IEnumerable<Action> GetActions(Agent agent)
        {
            return Variable.GenericSolver.GetActionsSatisfying(this, agent);
        }
    }
}
