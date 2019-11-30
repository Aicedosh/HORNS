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
            public PreconditionSet Preconditions { get; set; } = new PreconditionSet();
            public ActionPlannerNode Prev { get; set; }
            public Action PrevAction { get; set; }
            public Variable NeedState { get; set; }

            public ActionPlannerNode(float dist = float.MaxValue)
            {
                Distance = dist;
            }
        }
        
        // TODO: possibleGoals as param or field in agent?
        internal List<Action> Plan(Agent agent, IEnumerable<Action> idleActions, int possibleGoals = 5)
        {
            foreach (var action in agent.PossibleActions)
            {
                action.GetCost();      // cache costs
            }

            List<(INeed Need, float Priority)> needs = new List<(INeed, float)>();
            foreach (var need in agent.Needs)
            {
                if (!need.IsSatisfied())
                {
                    needs.Add((need, need.GetPriority()));
                }
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
                // TODO: optimize close
                var close = new List<PreconditionSet>();

                var goal = new ActionPlannerNode(0)
                {
                    NeedState = need.GetVariable().GetCopy()
                };

                foreach (var action in need.GetActionsTowards().Where(a=>agent.PossibleActions.Contains(a)))
                {
                    var nodeFromGoal = new ActionPlannerNode(action.CachedCost)
                    {
                        Prev = goal,
                        PrevAction = action,
                        NeedState = goal.NeedState.GetCopy()
                    };
                    action.ApplyResults(nodeFromGoal.NeedState);
                    open.Enqueue(nodeFromGoal, nodeFromGoal.Distance);
                }

                ActionPlannerNode last = null;
                while (open.Count > 0)
                {
                    var node = open.Dequeue();

                    // copy and update preconditions
                    var res = true;
                    foreach (var pre in node.Prev.Preconditions)
                    {
                        res = node.Preconditions.Add(pre.Clone());
                    }

                    node.PrevAction.SubtractResults(node.Preconditions);
                    node.Preconditions.RemoveWhere(x => x.IsFulfilled());
                    bool badPre = false;
                    foreach (var pre in node.PrevAction.GetPreconditions())
                    {
                        if (!node.Preconditions.Add(pre.Clone()))
                        {
                            badPre = true;
                            break;
                        }
                    }
                    if (badPre)
                    {
                        continue;
                    }

                    bool preLeft = false;
                    foreach (var pre in node.Preconditions)
                    {
                        if (!pre.IsFulfilled() && !pre.IsFulfilledByWorld())
                        {
                            preLeft = true;
                            break;
                        }
                    }
                    if (!preLeft)
                    {
                        last = node;
                        break;
                    }

                    bool cut = false;
                    foreach (var preSet in close)
                    {
                        cut = true;
                        // if preSet has a pre that we don't have then we're not equal/worse
                        // if we have a pre that's better than the same pre in preSet then we're not equal/worse
                        foreach (var pre in preSet)
                        {
                            Precondition refPre = pre;
                            if (!node.Preconditions.TryGet(ref refPre) || !refPre.IsEqualOrWorse(pre))
                            {
                                cut = false;
                                break;
                            }
                        }
                        if (cut)
                        {
                            break;
                        }
                    }
                    if (cut)
                    {
                        continue;
                    }

                    close.Add(node.Preconditions);
                    
                    foreach (var pre in node.Preconditions)
                    {
                        var actions = pre.GetActions();
                        foreach (var action in actions.Where(a => agent.PossibleActions.Contains(a)))
                        {
                            var needState = node.NeedState.GetCopy();
                            action.ApplyResults(needState);
                            if (need.EvaluateFor(node.NeedState) <= need.EvaluateFor(needState))
                            {
                                // copy as little as possible at this stage; copy the rest when we visit it
                                var newNode = new ActionPlannerNode(node.Distance + action.CachedCost)
                                {
                                    Prev = node,
                                    PrevAction = action,
                                    NeedState = needState
                                };

                                open.Enqueue(newNode, newNode.Distance);
                            }
                        }
                    }
                }
                
                if (last != null)
                {
                    var plan = new List<Action>();
                    while (last.PrevAction != null)
                    {
                        plan.Add(last.PrevAction);
                        last = last.Prev;
                    }

                    float cost = 0f;
                    IdSet<Variable> variables = new IdSet<Variable>();
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
