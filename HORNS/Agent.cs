﻿using System;
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
        private int currentAction = 0;

        //TODO: implement custom collection to allow for
        //  - easy collection copy
        //  - finding using Variable.Id (not implemented yet)
        //Maybe use decorator with HashSet<Variable> and custom comparer? or override Variable.GetHashCode and Variable.Equal?
        private ICollection<Variable> variables = new HashSet<Variable>();
        internal IEnumerable<Variable> Variables => variables;

        private ICollection<INeed> needs = new List<INeed>();
        public IEnumerable<INeed> Needs => needs;

        public void AddNeed<T>(Need<T> need) //Necessary to ensure only this implementation of the interface can be added to the list
        {
            needs.Add(need);
            variables.Add(need);
            variables.Add(need.Variable);
        }

        private ICollection<Action> possibleActions = new HashSet<Action>();
        internal ICollection<Action> PossibleActions => possibleActions; //TODO: Extract IImmutableSet<T> interface and implement via decorator?

        public void AddAction(Action action)
        {
            possibleActions.Add(action);
            foreach(Variable var in action.GetVariables())
            {
                variables.Add(var);
            }
        }

        public Action GetNextAction()
        {
            if(plannedActions.Count == currentAction)
            {
                //We have ran out of planned actions, recalculate
                RecalculateActions();
            }

            return plannedActions[currentAction++];
        }

        public void RecalculateActions()
        {
            plannedActions = new List<Action>(planner.Plan(this, Enumerable.Empty<Action>())); //TODO: Add idle actions
            currentAction = 0;
        }
    }
}