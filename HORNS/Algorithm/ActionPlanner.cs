using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HORNS
{
    internal class ActionPlanner
    {
        internal List<Action> Plan(Agent agent, IEnumerable<Action> idleActions)
        {
            return new List<Action> { agent.PossibleActions.First() };
        }
    }
}
