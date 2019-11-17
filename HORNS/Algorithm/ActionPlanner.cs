using System.Collections.Generic;
using System.Linq;
using Priority_Queue;

namespace HORNS
{
    internal class ActionPlanner
    {
        private class ActionPlannerNode
        {
            public float Distance { get; set; }
            // TODO: is Actions this actually needed? we're looking at actions per requirement anyway
            public List<Action> Actions { get; set; } = new List<Action>();
            public List<Requirement> Requirements { get; set; } = new List<Requirement>();
            public ActionPlannerNode Prev { get; set; }
            public Action PrevAction { get; set; }

            private VariableSet variablePatch = new VariableSet();

            public ActionPlannerNode(float dist = float.MaxValue)
            {
                Distance = dist;
            }
        }
        
        // TODO: possibleGoals as param or field in agent?
        // TODO: check for at least one idle action - here or somewhere else?
        internal List<Action> Plan(Agent agent, IEnumerable<Action> idleActions, int possibleGoals = 5)
        {
            foreach (var action in agent.PossibleActions)
            {
                action.GetCost();      // cache costs
            }

            List<(INeed Need, float Priority)> needs = new List<(INeed, float)>();
            foreach (var need in agent.Needs)
            {
                needs.Add((need, need.GetPriority()));
            }
            // lower "priority" => more important need
            needs.Sort((x, y) => x.Priority.CompareTo(y.Priority));     // TODO: this can be done faster
            if (needs.Count > possibleGoals)
            {
                needs.RemoveRange(possibleGoals, needs.Count - possibleGoals);
            }

            List<Action> bestPlan = new List<Action>();
            float bestCost = float.MaxValue;

            foreach (var (need, priority) in needs)
            {
                var open = new SimplePriorityQueue<ActionPlannerNode>();
                // TODO: figure out what the hell close is supposed to be
                //var close = new HashSet<ActionPlannerNode>();

                var goal = new ActionPlannerNode(0);
                open.Enqueue(goal, goal.Distance);      // TODO: something smarter than adding the same value twice

                ActionPlannerNode last = null;
                while(open.Count > 0)
                {
                    var node = open.Dequeue();
                    // TODO: add to close?
                    // TODO: stop condition (set last and break) - how to check we've reached current world state?

                    // TODO: also consider actions for need
                    // the shouldn't be precondition requirements since they don't have to be met fully
                    // consider adding NeedRequirement for consistency
                    foreach (var req in node.Requirements)
                    {
                        var actions = req.GetActions();
                        foreach (var action in actions)
                        {
                            // from Dumbledore:
                            //if (w in OPEN)
                            //{
                            //    odległość[w] = nieskończoność
                            //    wstaw w do OPEN
                            //}
                            //if (odległość[w] > odległość[u] + waga<u, w>)
                            //{
                            //    odległość[w] = odległość[u] + waga < u,w >
                            //    aktualizacja priorytetu wierzchołka w(w OPEN)
                            //    poprzedni[w] = u
                            //}

                            // TODO: check if already exists?

                        }
                    }
                }
                
                if (last != null)
                {
                    var plan = new List<Action>();
                    while (last != null)
                    {
                        plan.Add(last.PrevAction);
                        last = last.Prev;
                    }

                    // TODO: simulate list and update bestPlan if needed
                }
            }    
            
            foreach (var idle in idleActions)
            {
                if (idle.GetCost() < bestCost)
                {
                    bestPlan = new List<Action>() { idle };
                    bestCost = idle.CachedCost;
                }
            }

            return bestPlan;
        }
    }
}
