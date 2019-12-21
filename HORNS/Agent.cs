#if DEBUG
#define MEASURE_TIME
#endif

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HORNS
{
    /// <summary>
    /// Klasa reprezentująca agenta korzystającego z planowania akcji.
    /// </summary>
    public class Agent
    {
        //TODO: Should this be a field in Agent, an interface (with possibility to change by the developer) or
        //      should the Agent be simple object and planning actions should be responsibility of application's wrapper class
        private ActionPlanner planner = new ActionPlanner();
        private List<Action> plannedActions = new List<Action>();
        private bool shouldRecalculate = false;
        private System.Action<Agent> recalculateCallback = null;
        private int nextActionIdx = 0;

        /// <summary>
        /// Indeks obecnie wykonywanej akcji.
        /// </summary>
        public int CurrentAction => nextActionIdx - 1;
        /// <summary>
        /// Akcje zaplanowane do wykonania w obecnej chwili.
        /// </summary>
        public IEnumerable<Action> PlannedActions => plannedActions;
        /// <summary>
        /// Pozostała liczba akcji obecnego planu.
        /// </summary>
        public int PlannedActionsLeft => plannedActions.Count - nextActionIdx;
#if MEASURE_TIME
        /// <summary>
        /// Czas trwania ostatniego planowania.
        /// </summary>
        public TimeSpan LastPlanTime { get; private set; }
#endif

        internal IdSet<Variable> Variables { get; } = new IdSet<Variable>();

        internal IdSet<INeedInternal> NeedsInternal { get; } = new IdSet<INeedInternal>();
        /// <summary>
        /// Kolekcja potrzeb agenta.
        /// </summary>
        public IEnumerable<INeed> Needs => NeedsInternal;

        //Necessary to ensure only this implementation can be added to the list
        /// <summary>
        /// Dodaje potrzebę do kolekcji potrzeb agenta.
        /// </summary>
        /// <typeparam name="T">Typ zmiennej związanej z dodawaną potrzebą.</typeparam>
        /// <param name="need">Dodawana potrzeba.</param>
        public void AddNeed<T>(Need<T> need)
        {
            NeedsInternal.Add(need);
            Variables.Add(need.Variable);
        }

        private HashSet<Action> possibleActions = new HashSet<Action>();
        internal ISet<Action> PossibleActions => possibleActions;

        /// <summary>
        /// Dodaje akcję do możliwych akcji agenta.
        /// </summary>
        /// <param name="action">Dodawana akcja.</param>
        public void AddAction(Action action)
        {
            possibleActions.Add(action);
            foreach(Variable var in action.GetVariables())
            {
                Variables.Add(var);
            }
        }

        /// <summary>
        /// Dodaje wiele akcji do możliwych akcji agenta.
        /// </summary>
        /// <param name="actions">Dodawane akcje.</param>
        public void AddActions(params Action[] actions)
        {
            foreach (var action in actions)
            {
                AddAction(action);
            }
        }

        internal List<Action> IdleActions = new List<Action>();

        /// <summary>
        /// Dodaje akcję do możliwych akcji bezczynności agenta.
        /// </summary>
        /// <param name="action">Dodawana akcja.</param>
        public void AddIdleAction(Action action)
        {
            IdleActions.Add(action);
            // TODO: should idles have requirements?
            foreach (Variable var in action.GetVariables())
            {
                Variables.Add(var);
            }
        }

        /// <summary>
        /// Dodaje wiele akcji do możliwych akcji bezczynności agenta.
        /// </summary>
        /// <param name="actions">Dodawane akcje.</param>
        public void AddIdleActions(params Action[] actions)
        {
            foreach (var action in actions)
            {
                AddIdleAction(action);
            }
        }

        /// <summary>
        /// Wymusza przeliczenie planu przy następnym pobraniu akcji.
        /// </summary>
        public void ForceRecalculate()
        {
            shouldRecalculate = true;
        }

        /// <summary>
        /// Określa akcję do wykonania po przeliczeniu planu.
        /// </summary>
        /// <param name="callback">Akcja do wykonania.</param>
        public void SetRecalculateCallback(System.Action<Agent> callback)
        {
            recalculateCallback = callback;
        }

        /// <summary>
        /// Wyznacza kolejną akcję do wykonania.
        /// </summary>
        /// <returns>Akcja do wykonania.</returns>
        public Action GetNextAction()
        {
            if(shouldRecalculate || plannedActions.Count == nextActionIdx)
            {
                shouldRecalculate = false;
                RecalculateActions();
            }

            if (plannedActions.Count == 0)
            {
                return null;
            }
            return plannedActions[nextActionIdx++];
        }

        /// <summary>
        /// Wyznacza kolejną akcję do wykonania w sposób asynchroniczny.
        /// </summary>
        /// <param name="token">Token umożliwiający przerwanie planowania w trakcie.</param>
        /// <returns>Akcja do wykonania.</returns>
        public async Task<Action> GetNextActionAsync(CancellationToken? token = null)
        {
            if(token.HasValue && token.Value.IsCancellationRequested)
            {
                return null;
            }

            if (shouldRecalculate || plannedActions.Count == nextActionIdx)
            {
                shouldRecalculate = false;
                await RecalculateActionsAsync(token);
            }

            if (plannedActions.Count == 0)
            {
                return null;
            }
            return plannedActions[nextActionIdx++];
        }

        /// <summary>
        /// Wyznacza plan akcji do wykonania.
        /// </summary>
        public void RecalculateActions()
        {
#if MEASURE_TIME
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif
            plannedActions = planner.Plan(this);
            nextActionIdx = 0;
#if MEASURE_TIME
            sw.Stop();
            LastPlanTime = sw.Elapsed;
#endif
            recalculateCallback?.Invoke(this);
        }

        /// <summary>
        /// Wyznacza plan akcji do wykonania w sposób asynchroniczny.
        /// </summary>
        /// <param name="token">Token umożliwiający przerwanie planowania w trakcie.</param>
        /// <returns></returns>
        public async Task RecalculateActionsAsync(CancellationToken? token = null)
        {
#if MEASURE_TIME
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif
            plannedActions = await Task.Run(() => planner.Plan(this, true, token));
            nextActionIdx = 0;
#if MEASURE_TIME
            sw.Stop();
            LastPlanTime = sw.Elapsed;
#endif
            recalculateCallback?.Invoke(this);
        }
    }
}
