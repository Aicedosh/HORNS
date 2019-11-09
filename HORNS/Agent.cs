using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class Agent
    {
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
    }
}
