using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework.AI.GOAP
{
    public class Goal
    {
        public Dictionary<string, int> Sgoals;
        public bool Remove;

        public Goal(string s, int i, bool r)
        {
            Sgoals = new Dictionary<string, int>();
            Sgoals.Add(s, i);
            Remove = r;
        }
    }

    public class GAgent : MonoBehaviour
    {
        public List<GAction> Actions = new List<GAction>();
        public Dictionary<Goal, int> Goals = new Dictionary<Goal, int>();
        public WorldStates Beliefs = new WorldStates();

        private GPlanner planner;
        private Queue<GAction> actionQueue;
        public GAction currentAction;
        private Goal currentGoal;
        private bool invoked;

        // Start is called before the first frame update
        public void Start()
        {
            var acts = GetComponents<GAction>();

            foreach (GAction a in acts)
                Actions.Add(a);
        }

        private void CompleteAction()
        {
            currentAction.running = false;
            currentAction.PostPerform();
            invoked = false;
        }

        private void LateUpdate()
        {
            if (currentAction != null && currentAction.running)
            {
                if (currentAction.agent.hasPath && currentAction.agent.remainingDistance < 1f)
                {
                    if (invoked) return;
                    Invoke(nameof(CompleteAction), currentAction.duration);
                    invoked = true;
                }

                return;
            }

            if (planner == null || actionQueue == null)
            {
                planner = new GPlanner();

                var sortedGoals = Goals.OrderByDescending(entry => entry.Value);

                foreach (var sg in sortedGoals)
                {
                    actionQueue = planner.plan(Actions, sg.Key.Sgoals, null);
                    if (actionQueue == null)
                        continue;

                    currentGoal = sg.Key;
                    break;
                }
            }

            if (actionQueue != null && actionQueue.Count == 0)
            {
                if (currentGoal.Remove)
                    Goals.Remove(currentGoal);
                planner = null;
            }

            if (actionQueue == null || actionQueue.Count <= 0)
                return;

            currentAction = actionQueue.Dequeue();

            if (currentAction.PrePerform())
            {
                if (currentAction.target == null && currentAction.targetTag != string.Empty)
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);

                if (currentAction.target == null)
                    return;

                currentAction.running = true;
                currentAction.agent.SetDestination(currentAction.target.transform.position);
            }
            else
            {
                actionQueue = null;
            }
        }
    }
}
