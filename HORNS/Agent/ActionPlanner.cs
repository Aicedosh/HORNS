﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Priority_Queue;

using NeedList = System.Collections.Generic.List<(HORNS.INeedInternal Need, float Priority)>;
using VariableSet = HORNS.IdSet<HORNS.Variable>;
using OpenSet = Priority_Queue.SimplePriorityQueue<HORNS.ActionPlanner.ActionPlannerNode>;
// TODO: optimize close
using CloseSet = System.Collections.Generic.List<HORNS.PreconditionSet>;

namespace HORNS
{
    internal class ActionPlanner
    {
        internal class ActionPlannerNode
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
        
        internal (List<Action> actions, INeed need) Plan(Agent agent, bool useSnapshot = false,
                                                         CancellationToken? token = null)
        {
            VariableSet variableSet = GetVariableSet(agent, useSnapshot);
            CacheAgentActionsCosts(agent, variableSet);
            NeedList needs = GetUnsatisfiedNeeds(agent, variableSet);

            List<Action> bestPlan = new List<Action>();
            float bestCost = float.MaxValue;
            INeed bestNeed = null;

            foreach (var (need, priority) in needs)
            {
                var open = new OpenSet();

                if (need != null)
                {
                    var needState = need.GetVariable().GetCopy();
                    if (variableSet != null)
                    {
                        variableSet.TryGet(ref needState);
                    }
                    var goal = new ActionPlannerNode(0)
                    {
                        NeedState = needState
                    };

                    foreach (var action in need.GetActionsTowards(agent))
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
                }
                else // "null need" - idle
                {
                    var dummy = new ActionPlannerNode(0);
                    foreach (var idle in agent.IdleActions)
                    {
                        var nodeIdle = new ActionPlannerNode(idle.CachedCost)
                        {
                            Prev = dummy,
                            PrevAction = idle
                        };
                        open.Enqueue(nodeIdle, nodeIdle.Distance);
                    }
                }

                ActionPlannerNode last = ComputePath(agent, open, variableSet, token, need);
                
                if (last != null)
                {
                    var plan = new List<Action>();
                    while (last.PrevAction != null)
                    {
                        plan.Add(last.PrevAction);
                        last = last.Prev;
                    }

                    float cost = 0f;
                    IdSet<Variable> variables;
                    if (variableSet != null)
                    {
                        variables = variableSet.Clone();
                    }
                    else
                    {
                        variables = new IdSet<Variable>();
                    }
                    foreach (var action in plan)
                    {
                        cost += action.GetCost(agent, variables);
                        action.ApplyResults(variables);
                    }
                    if (cost < bestCost)
                    {
                        bestPlan = plan;
                        bestCost = cost;
                        bestNeed = need;
                    }
                }
            }

            return (bestPlan, bestNeed);
        }

        private VariableSet GetVariableSet(Agent agent, bool useSnapshot)
        {
            if (useSnapshot)
            {
                Variable.VariableLock.EnterReadLock();
                VariableSet variableSet = agent.Variables.Clone();
                Variable.VariableLock.ExitReadLock();
                return variableSet;
            }
            return null;
        }

        private void CacheAgentActionsCosts(Agent agent, VariableSet variableSet)
        {
            foreach (var action in agent.PossibleActions)
            {
                action.GetCost(agent, variableSet);
            }

            foreach (var action in agent.IdleActions)
            {
                action.GetCost(agent, variableSet);
            }
        }

        private NeedList GetUnsatisfiedNeeds(Agent agent, VariableSet variableSet)
        {
            NeedList needs = new NeedList();
            if (agent.NeedsToCalculate != 0)
            {
                foreach (var need in agent.NeedsInternal)
                {
                    if (!need.IsSatisfied(variableSet))
                    {
                        needs.Add((need, need.EvaluateFor(variableSet)));
                    }
                }
                // lower "priority" => more important need
                needs.Sort((x, y) => x.Priority.CompareTo(y.Priority));     // TODO: optimize
                if (agent.NeedsToCalculate > 0 && needs.Count > agent.NeedsToCalculate)
                {
                    needs.RemoveRange(agent.NeedsToCalculate, needs.Count - agent.NeedsToCalculate);
                }
            }
            needs.Add((null, 0));
            return needs;
        }

        private bool AddActionPreconditions(ActionPlannerNode node)
        {
            foreach (var pre in node.PrevAction.GetPreconditions())
            {
                if (!node.Preconditions.Add(pre.Clone()))
                {
                    return false;
                }
            }
            return true;
        }

        private bool AllPreconditionsSatisfied(ActionPlannerNode node, VariableSet variableSet)
        {
            foreach (var pre in node.Preconditions)
            {
                if (!pre.IsFulfilled() && !pre.IsFulfilledBy(variableSet))
                {
                    return false;
                }
            }
            return true;
        }

        private bool ShouldCutNode(ActionPlannerNode node, CloseSet close)
        {
            foreach (var preSet in close)
            {
                bool cut = true;
                // if preSet has a pre that we don't have then we're not equal/worse
                // if we have a refPre that's better than the same pre in preSet then we're not equal/worse
                foreach (var pre in preSet)
                {
                    Precondition refPre = pre;
                    if (!node.Preconditions.TryGet(ref refPre) || refPre.IsBetterThan(pre) != Precondition.ComparisonResult.EqualWorse)
                    {
                        cut = false;
                        break;
                    }
                }
                if (cut)
                {
                    return true;
                }
            }
            return false;
        }

        private ActionPlannerNode ComputePath(Agent agent, OpenSet open, VariableSet variableSet, CancellationToken? token, INeedInternal need)
        {
            var close = new CloseSet();
            while (open.Count > 0)
            {
                if (token.HasValue && token.Value.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                var node = open.Dequeue();

                // copy and update preconditions
                var res = true;
                foreach (var pre in node.Prev.Preconditions)
                {
                    res = node.Preconditions.Add(pre.Clone());
                }

                if (!node.PrevAction.ApplyResults(node.Preconditions))
                {
                    continue;
                }
                node.Preconditions.RemoveWhere(x => x.IsFulfilled());

                if (!AddActionPreconditions(node))
                {
                    continue;
                }

                if (AllPreconditionsSatisfied(node, variableSet))
                {
                    return node;
                }

                if (ShouldCutNode(node, close))
                {
                    continue;
                }

                close.Add(node.Preconditions);

                foreach (var pre in node.Preconditions)
                {
                    var actions = pre.GetActions(agent);
                    foreach (var action in actions)
                    {
                        Variable newNeedState = null;
                        float n1 = 0, n2 = 0;
                        if (need != null)
                        {
                            newNeedState = node.NeedState?.GetCopy();
                            action.ApplyResults(newNeedState);
                            n1 = need.EvaluateFor(node.NeedState);
                            n2 = need.EvaluateFor(newNeedState);
                        }

                        if (n1 <= n2)
                        {
                            // copy as little as possible at this stage; copy the rest when we visit it
                            var newNode = new ActionPlannerNode(node.Distance + action.CachedCost)
                            {
                                Prev = node,
                                PrevAction = action,
                                NeedState = newNeedState
                            };

                            open.Enqueue(newNode, newNode.Distance);
                        }
                    }
                }
            }
            return null;
        }
    }
}
