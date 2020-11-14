using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework.AI.GOAP
{
    public class ActionNode
    {
        public readonly ActionNode Parent;
        public readonly float Cost;
        public readonly Dictionary<string, int> State;
        public readonly GAction Action;

        public ActionNode(ActionNode parent, float cost, Dictionary<string, int> allstates, GAction action)
        {
            Parent = parent;
            Cost = cost;
            State = new Dictionary<string, int>(allstates);
            Action = action;
        }
    }

    public class GPlanner
    {
        public Queue<GAction> plan(List<GAction> actions, Dictionary<string, int> goal, WorldStates states)
        {
            var usableActions = actions.Where(a => a.IsAchievable()).ToList();

            var leaves = new List<ActionNode>();
            var start = new ActionNode(null, 0, GWorld.Instance.GetWorld().GetStates(), null);
            var success = BuildGraph(start, leaves, usableActions, goal);

            if (!success)
            {
                Debug.Log("NO PLAN");
                return null;
            }

            ActionNode cheapest = null;

            foreach (var leaf in leaves)
            {
                if (cheapest == null)
                {
                    cheapest = leaf;
                }
                else
                {
                    if (leaf.Cost < cheapest.Cost)
                        cheapest = leaf;
                }
            }

            var result = new List<GAction>();
            var n = cheapest;
            while (n != null)
            {
                if (n.Action != null)
                {
                    result.Insert(0, n.Action);
                }

                n = n.Parent;
            }

            var queue = new Queue<GAction>();
            foreach (var a in result)
                queue.Enqueue(a);


#if UNITY_EDITOR
            Debug.Log("The Plan is: ");
            foreach (var a in queue)
                Debug.Log("Q: " + a.actionName);
#endif

            return queue;
        }

        private bool BuildGraph(ActionNode parent, ICollection<ActionNode> leaves, IReadOnlyCollection<GAction> usableActions,
            Dictionary<string, int> goal)
        {
            var foundPath = false;
            var currentState = new Dictionary<string, int>(parent.State);

            foreach (var action in usableActions)
            {
                if (!action.IsAchievableGiven(parent.State))
                    continue;

                foreach (var eff in action.effects)
                {
                    if (!currentState.ContainsKey(eff.Key))
                        currentState.Add(eff.Key, eff.Value);
                }

                var node = new ActionNode(parent, parent.Cost + action.cost, currentState, action);

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

            return foundPath;
        }

        private bool GoalAchieved(Dictionary<string, int> goal, IReadOnlyDictionary<string, int> state)
        {
            return goal.All(g => state.ContainsKey(g.Key));
        }

        private List<GAction> ActionSubset(IEnumerable<GAction> actions, GAction removeMe)
        {
            return actions.Where(a => !a.Equals(removeMe)).ToList();
        }
    }
}
