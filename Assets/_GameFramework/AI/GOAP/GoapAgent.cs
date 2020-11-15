using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework.AI.GOAP
{
    public class Goal
    {
        public Dictionary<string, int> Sgoals;
        public readonly bool Remove;

        public Goal(string s, int i, bool r)
        {
            Sgoals = new Dictionary<string, int>();
            Sgoals.Add(s, i);
            Remove = r;
        }
    }

    public class GoapAgent : MonoBehaviour
    {
        [SerializeField] private List<GoapAction> _actions = new List<GoapAction>();
        [SerializeField] private GoapAction CurrentAction;

        private Dictionary<Goal, int> _goals = new Dictionary<Goal, int>();
        private GoapPlanner _planner;
        private Queue<GoapAction> _actionsQueue;
        private Goal _currentGoal;
        private bool _invoked;

        public void Start()
        {
            var acts = GetComponents<GoapAction>();

            foreach (var a in acts)
                _actions.Add(a);
        }

        public void AddGoal(Goal goal, int goalCost)
        {
            _goals.Add(goal, goalCost);
        }

        private void CompleteAction()
        {
            CurrentAction.Running = false;
            CurrentAction.PostPerform();

            _invoked = false;
        }

        private void LateUpdate()
        {
            ///TODO - if(Action Running);
            if (CurrentAction != null && CurrentAction.Running)
            {
                ///TODO - if(Action.HasComplete) -> Action.CompleteAction();
                if (CurrentAction.agent.hasPath && CurrentAction.agent.remainingDistance < 1f)
                {
                    if (_invoked) return;
                    Invoke(nameof(CompleteAction), CurrentAction.Duration);
                    _invoked = true;
                }

                return;
            }

            //TODO - Get PriorityGoal
            if (_planner == null || _actionsQueue == null)
            {
                _planner = new GoapPlanner();

                var sortedGoals = _goals.OrderByDescending(entry => entry.Value);

                foreach (var sg in sortedGoals)
                {
                    _actionsQueue = _planner.GetPlan(_actions, sg.Key.Sgoals, null);
                    if (_actionsQueue == null)
                        continue;

                    _currentGoal = sg.Key;
                    break;
                }
            }

            //TODO - Remove _goals
            if (_actionsQueue != null && _actionsQueue.Count == 0)
            {
                if (_currentGoal.Remove)
                    _goals.Remove(_currentGoal);
                _planner = null;
            }

            if (_actionsQueue == null || _actionsQueue.Count <= 0)
                return;

            CurrentAction = _actionsQueue.Dequeue();

            /// TODO - if Action can push - Start Action
            if (CurrentAction.PrePerform())
            {
                if (CurrentAction.Target == null && CurrentAction.TargetTag != string.Empty)
                    CurrentAction.Target = GameObject.FindWithTag(CurrentAction.TargetTag);

                if (CurrentAction.Target == null)
                    return;

                CurrentAction.Running = true;
                CurrentAction.agent.SetDestination(CurrentAction.Target.transform.position);
            }
            else
            {
                _actionsQueue = null;
            }
        }
    }
}
