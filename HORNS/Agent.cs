#if DEBUG
#define MEASURE_TIME
#endif

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HORNS
{
    public class Agent
    {
        //TODO: Should this be a field in Agent, an interface (with possibility to change by the developer) or
        //      should the Agent be simple object and planning actions should be responsibility of application's wrapper class
        private ActionPlanner planner = new ActionPlanner();
        private List<Action> plannedActions = new List<Action>();
        private List<Action> idleActions = new List<Action>();
        private int currentAction = 0;
        internal IdSet<Variable> Variables { get; } = new IdSet<Variable>();

        internal IdSet<INeedInternal> NeedsInternal { get; } = new IdSet<INeedInternal>();
        public IEnumerable<INeed> Needs => NeedsInternal;

        public void AddNeed<T>(Need<T> need) //Necessary to ensure only this implementation of the interface can be added to the list
        {
            NeedsInternal.Add(need);
            // TODO: think about this
            //Variables.Add(need);
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

        public Action GetNextAction()
        {
            if(plannedActions.Count == currentAction)
            {
                //We have ran out of planned actions, recalculate
                RecalculateActions();
            }

            if (plannedActions.Count == 0)
            {
                return null;
            }
            return plannedActions[currentAction++];
        }

        public async Task<Action> GetNextActionAsync()
        {
            if (plannedActions.Count == currentAction)
            {
                await RecalculateActionsAsync();
            }

            if (plannedActions.Count == 0)
            {
                return null;
            }
            return plannedActions[currentAction++];
        }

        public void RecalculateActions()
        {
#if MEASURE_TIME
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif
            plannedActions = planner.Plan(this, idleActions);
            currentAction = 0;
#if MEASURE_TIME
            sw.Stop();
            Console.WriteLine($"[DEBUG] Planning took {sw.Elapsed}");
#endif
        }

        public async Task RecalculateActionsAsync()
        {
#if MEASURE_TIME
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif
            plannedActions = await Task.Run(() => planner.Plan(this, idleActions, true));
            currentAction = 0;
#if MEASURE_TIME
            sw.Stop();
            Console.WriteLine($"[DEBUG] Planning took {sw.Elapsed}");
#endif
        }
    }
}
