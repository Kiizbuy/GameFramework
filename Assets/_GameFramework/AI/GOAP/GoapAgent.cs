using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.AI.GOAP
{
    public class SubGoal
    {
        public Dictionary<string, int> SubGoals;
        public bool Remove;

        public SubGoal(string action, int value, bool remove)
        {
            SubGoals = new Dictionary<string, int>();
            SubGoals.Add(action, value);
            Remove = remove;
        }
    }

    public class GoapAgent : MonoBehaviour
    {
        public List<GoapAction> Actions;
        public Dictionary<SubGoal, int> Goals = new Dictionary<SubGoal, int>();
        public GoapAction CurrentAction;

        private GoapPlanner _planner;
        private Queue<GoapAction> _actionsQueue = new Queue<GoapAction>();
        private SubGoal _currentGoal;

        // Start is called before the first frame update
        void Start()
        {
            var actions = GetComponents<GoapAction>();
            foreach (var a in actions)
            {
                Actions.Add(a);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
