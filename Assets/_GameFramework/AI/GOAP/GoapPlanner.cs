using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.AI.GOAP
{
    public class ActionNode
    {
        public ActionNode Parent;
        public float Cost;
        public Dictionary<string, int> State;
        public GoapAction Action;

        public ActionNode(ActionNode parent, float cost, IDictionary<string, int> allStates, GoapAction action)
        {
            Parent = parent;
            Cost = cost;
            State = new Dictionary<string, int>(allStates);
            Action = action;
        }
    }

    public class GoapPlanner
    {
        public Queue<GoapAction> GetPlan(ICollection<GoapAction> actions, IDictionary<string, int> goal,
            GoapWorldState state)
        {
            var usableActions = new List<GoapAction>();

            foreach (var currentAction in actions)
                if (currentAction.IsAchievable)
                    usableActions.Add(currentAction);

            var leavesActionNode = new List<ActionNode>();
            var startNode = new ActionNode(null, 0, new Dictionary<string, int>(), null);

            var success = BuildGraph(startNode, leavesActionNode, usableActions, goal);

            if (!success)
            {
                Debug.Log("Can't create plan");
                return null;
            }

            ActionNode cheapestNode = null;

            foreach (var leaf in leavesActionNode)
            {
                if (cheapestNode == null)
                    cheapestNode = leaf;
                else if (leaf.Cost < cheapestNode.Cost)
                {
                    cheapestNode = leaf;
                }
            }

            var result = new List<GoapAction>();
            ActionNode n = cheapestNode;

            while (n != null)
            {
                if (n.Action != null)
                {
                    result.Insert(0, n.Action);
                }

                n = n.Parent;
            }

            var queue = new Queue<GoapAction>();

            foreach (var action in result)
                queue.Enqueue(action);

            return queue;
        }

        private bool BuildGraph(ActionNode parent, ICollection<ActionNode> leaves,
            ICollection<GoapAction> usableActions, IDictionary<string, int> goal)
        {
            var foundPath = false;

            foreach (var action in usableActions)
            {
                if (action.IsAchievableHasGiven(parent.State))
                {
                    var currentState = new Dictionary<string, int>(parent.State);

                    foreach (var effects in action.AfterEffects)
                    {
                        if (!currentState.ContainsKey(effects.Key))
                            currentState.Add(effects.Key, effects.Value);
                    }

                    var node = new ActionNode(parent, parent.Cost + action.ActionCost, currentState, action);
                    if (GoalAchieved(goal, currentState))
                    {
                        leaves.Add(node);
                        foundPath = true;
                    }
                    else
                    {
                        var subset = ActionSubset(usableActions, action);
                        var found = BuildGraph(node, leaves, subset, goal);

                        if (found)
                            foundPath = true;
                    }

                }
            }

            return foundPath;
        }

        private bool GoalAchieved(IDictionary<string, int> goal, IDictionary<string, int> state)
        {
            foreach (var currentGoal in goal)
            {
                if (!state.ContainsKey(currentGoal.Key))
                    return false;
            }

            return true;
        }

        private IList<GoapAction> ActionSubset(ICollection<GoapAction> actions, GoapAction removableAction)
        {
            var subset = new List<GoapAction>();

            foreach (var a in actions)
            {
                if (!a.Equals(removableAction))
                    subset.Add(a);
            }

            return subset;
        }
    }
}
