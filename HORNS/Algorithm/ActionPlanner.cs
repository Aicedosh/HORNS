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
            // TODO: is Actions actually needed? we're looking at actions per requirement anyway
            // public List<Action> Actions { get; set; } = new List<Action>();
            // TODO: [A] structure to store requirements - something like VariableSet?
            public HashSet<Requirement> Requirements { get; set; } = new HashSet<Requirement>();
            public ActionPlannerNode Prev { get; set; }
            public Action PrevAction { get; set; }
            // public VariableSet VariablePatch { get; set; }

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
            needs.Sort((x, y) => x.Priority.CompareTo(y.Priority));     // TODO: optimize
            if (needs.Count > possibleGoals)
            {
                needs.RemoveRange(possibleGoals, needs.Count - possibleGoals);
            }

            List<Action> bestPlan = new List<Action>();
            float bestCost = float.MaxValue;

            foreach (var (need, priority) in needs)
            {
                var open = new SimplePriorityQueue<ActionPlannerNode>();
                // TODO: what to do about close? compare fulfilled/unfulfilled requirements?
                //var close = new HashSet<ActionPlannerNode>();

                var goal = new ActionPlannerNode(0);
                foreach (var action in need.GetActionsTowards())
                {
                    var nodeFromGoal = new ActionPlannerNode(action.CachedCost);
                    nodeFromGoal.Prev = goal;
                    nodeFromGoal.PrevAction = action;
                    open.Enqueue(nodeFromGoal, nodeFromGoal.Distance);  // TODO: something smarter than adding the same value twice
                }

                ActionPlannerNode last = null;
                while(open.Count > 0)
                {
                    var node = open.Dequeue();
                    // TODO: add to close?

                    if (node.Prev != null)  // TODO: unnecessary check if we're moving goal's iteration outside
                    {
                        bool reqLeft = false;
                        foreach (var req in node.Prev.Requirements)
                        {
                            node.Requirements.Add(req.Clone());
                        }
                        foreach (var pre in node.PrevAction.GetPreconditions())
                        {
                            node.Requirements.Add(pre.GetRequirement());
                        }

                        // TODO: [A] please make sure this can be done!
                        node.PrevAction.SubtractResults(node.Requirements);
                        foreach (var req in node.Requirements)
                        {
                            // TODO: move fulfilled to another set? after we figure out what to do abt partial fulfillment
                            if (!req.Fulfilled && !req.IsFulfilled(agent.Variables))
                            {
                                reqLeft = true;
                                break;
                            }
                        }

                        if (!reqLeft)
                        {
                            last = node;
                            break;
                        }
                    }
                    
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
                            var newNode = new ActionPlannerNode(node.Distance + action.CachedCost);
                            // copy as little as possible at this stage; copy the rest when we visit it
                            newNode.Prev = node;
                            newNode.PrevAction = action;

                            open.Enqueue(newNode, newNode.Distance);
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

                    float cost = 0f;
                    VariableSet variables = new VariableSet();
                    foreach (var action in plan)
                    {
                        cost += action.GetCost(variables);
                        action.ApplyResults(variables);
                    }
                    cost *= priority;
                    if (cost < bestCost)
                    {
                        bestPlan = plan;
                        bestCost = cost;
                    }
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
