using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private ICollection<INeed> needs = new List<INeed>();
        public IEnumerable<INeed> Needs => needs;

        public void AddNeed<T>(Need<T> need) //Necessary to ensure only this implementation of the interface can be added to the list
        {
            needs.Add(need);
            Variables.Add(need);
            Variables.Add(need.Variable);
        }

        private ICollection<Action> possibleActions = new HashSet<Action>();
        internal ICollection<Action> PossibleActions => possibleActions; //TODO: Extract IImmutableSet<T> interface and implement via decorator?

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

        public void RecalculateActions()
        {
            plannedActions = new List<Action>(planner.Plan(this, idleActions));
            currentAction = 0;
        }
    }
}
