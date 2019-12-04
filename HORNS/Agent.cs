#if DEBUG
#define MEASURE_TIME
#endif

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HORNS
{
    public class Agent
    {
        //TODO: Should this be a field in Agent, an interface (with possibility to change by the developer) or
        //      should the Agent be simple object and planning actions should be responsibility of application's wrapper class
        private ActionPlanner planner = new ActionPlanner();
        private List<Action> plannedActions = new List<Action>();
        private bool shouldRecalculate = false;
        private System.Action<Agent> recalculateCallback = null;

        public int CurrentAction { get; private set; } = 0;
        public int PlannedActions => plannedActions.Count;
        public int PlannedActionsLeft => plannedActions.Count - CurrentAction;
#if MEASURE_TIME
        public TimeSpan LastPlanTime { get; private set; }
#endif

        internal IdSet<Variable> Variables { get; } = new IdSet<Variable>();

        internal IdSet<INeedInternal> NeedsInternal { get; } = new IdSet<INeedInternal>();
        public IEnumerable<INeed> Needs => NeedsInternal;

        public void AddNeed<T>(Need<T> need) //Necessary to ensure only this implementation of the interface can be added to the list
        {
            NeedsInternal.Add(need);
            Variables.Add(need.Variable);
        }

        private HashSet<Action> possibleActions = new HashSet<Action>();
        internal ISet<Action> PossibleActions => possibleActions;

        public void AddAction(Action action)
        {
            possibleActions.Add(action);
            foreach(Variable var in action.GetVariables())
            {
                Variables.Add(var);
            }
        }

        public void AddActions(params Action[] actions)
        {
            foreach (var action in actions)
            {
                AddAction(action);
            }
        }

        private List<Action> idleActions = new List<Action>();

        public void AddIdleAction(Action action)
        {
            idleActions.Add(action);
            // TODO: should idles have requirements?
            foreach (Variable var in action.GetVariables())
            {
                Variables.Add(var);
            }
        }

        public void AddIdleActions(params Action[] actions)
        {
            foreach (var action in actions)
            {
                AddIdleAction(action);
            }
        }

        public void ForceRecalculate()
        {
            shouldRecalculate = true;
        }

        public void SetRecalculateCallback(System.Action<Agent> callback)
        {
            recalculateCallback = callback;
        }

        public Action GetNextAction()
        {
            if(shouldRecalculate || plannedActions.Count == CurrentAction)
            {
                shouldRecalculate = false;
                RecalculateActions();
            }

            if (plannedActions.Count == 0)
            {
                return null;
            }
            return plannedActions[CurrentAction++];
        }

        public async Task<Action> GetNextActionAsync(CancellationToken? token = null)
        {
            if(token.HasValue && token.Value.IsCancellationRequested)
            {
                return null;
            }

            if (shouldRecalculate || plannedActions.Count == CurrentAction)
            {
                shouldRecalculate = false;
                await RecalculateActionsAsync(token);
            }

            if (plannedActions.Count == 0)
            {
                return null;
            }
            return plannedActions[CurrentAction++];
        }

        public void RecalculateActions()
        {
#if MEASURE_TIME
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif
            plannedActions = planner.Plan(this, idleActions);
            CurrentAction = 0;
#if MEASURE_TIME
            sw.Stop();
            LastPlanTime = sw.Elapsed;
#endif
            recalculateCallback?.Invoke(this);
        }

        public async Task RecalculateActionsAsync(CancellationToken? token = null)
        {
#if MEASURE_TIME
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif
            plannedActions = await Task.Run(() => planner.Plan(this, idleActions, true, token));
            CurrentAction = 0;
#if MEASURE_TIME
            sw.Stop();
            LastPlanTime = sw.Elapsed;
#endif
            recalculateCallback?.Invoke(this);
        }
    }
}
