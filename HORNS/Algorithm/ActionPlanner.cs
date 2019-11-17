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
            public List<Action> Actions { get; set; } = new List<Action>();
            public List<RequirementSet> Requirements { get; set; } = new List<RequirementSet>();
            public ActionPlannerNode Prev { get; set; }
            public Action PrevAction { get; set; }
            public VariableSet VariablePatch { get; set; }

            public ActionPlannerNode(float dist = float.MaxValue)
            {
                Distance = dist;
            }
        }

        private class RequirementSet
        {
            public Action Action { get; set; }
            public List<Requirement> Requirements { get; set; } = new List<Requirement>();
            public bool Closed { get; private set; } = false;

            public RequirementSet(Action action, VariableSet variables)
            {
                Action = action;
                foreach (var pre in action.GetPreconditions())
                {
                    Requirements.Add(pre.GetRequirement(variables));
                }
            }

            public RequirementSet(RequirementSet set)
            {
                Action = set.Action;
                foreach (var req in set.Requirements)
                {
                    Requirements.Add(req.Clone());
                }
            }

            public bool IsClosed()
            {
                //Closed = false;
                //foreach (var req in Requirements)
                //{
                    
                //}
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
                // TODO: are we using close?
                //var close = new HashSet<ActionPlannerNode>();

                var goal = new ActionPlannerNode(0);
                goal.VariablePatch = new VariableSet();
                open.Enqueue(goal, goal.Distance);      // TODO: something smarter than adding the same value twice

                ActionPlannerNode last = null;
                while(open.Count > 0)
                {
                    var node = open.Dequeue();

                    // TODO: add to close?
                    bool reqLeft = false;
                    if (node.Prev != null)
                    {
                        node.VariablePatch = node.Prev.VariablePatch.Clone();
                        foreach (var reqSet in node.Prev.Requirements)
                        {
                            node.Requirements.Add(new RequirementSet(reqSet));
                        }
                        node.Requirements.Add(new RequirementSet(node.PrevAction, node.VariablePatch));

                        foreach (var reqSet in node.Requirements)
                        {

                        }
                    }

                    // TODO: also consider actions for need
                    foreach (var reqItem in node.Requirements)
                    {
                        for (int i = 0; i < reqItem.Requirements.Count; i++)
                        {
                            var req = reqItem.Requirements[i];
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
                                // copy as little as possible on this stage; copy the rest when we visit it
                                newNode.Prev = node;
                                newNode.PrevAction = action;

                                open.Enqueue(newNode, newNode.Distance);

                            }
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
