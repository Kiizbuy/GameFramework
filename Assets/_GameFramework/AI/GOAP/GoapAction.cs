using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GameFramework.AI.GOAP
{
    public abstract class GoapAction : MonoBehaviour
    {
        //TODO - MOVE TO ANOTHER RESPONSIBILITY
        public GameObject Target;
        public string TargetTag;
        public NavMeshAgent agent;

        public bool Running = false; // TODO Remove that shit

        [SerializeField] private string _actionName = "Action";
        [SerializeField] private float _actionCost = 1f;
        [SerializeField] private float _actionDuration = 1f;
        [SerializeField] private WorldState[] _preConditionsSettings;
        [SerializeField] private WorldState[] _effectsSettings;

        private Dictionary<string, int> _preconditions = new Dictionary<string, int>();
        private Dictionary<string, int> _effects = new Dictionary<string, int>();

        public IReadOnlyDictionary<string, int> Preconditions => _preconditions;
        public IReadOnlyDictionary<string, int> Effects => _effects;
        public string ActionName => _actionName;
        public float Cost => _actionCost;
        public float Duration => _actionDuration;


        public void Awake()
        {
            agent = gameObject.GetComponent<NavMeshAgent>();

            InitActions();
        }

        private void InitActions()
        {
            foreach (var w in _preConditionsSettings)
            {
                _preconditions.Add(w.key, w.value);
            }

            foreach (var w in _effectsSettings)
            {
                _effects.Add(w.key, w.value);
            }
        }

        public virtual bool IsAchievable()
        {
            return true;
        }


        public bool IsAchievableGiven(Dictionary<string, int> conditions)
        {
            foreach (var p in _preconditions)
            {
                if (!conditions.ContainsKey(p.Key))
                    return false;
            }

            return true;
        }

        public abstract bool CanStartAction();
        public abstract bool ActionHasRunning();
        public abstract bool ActionHasComplete();
        public abstract bool PrePerform();
        public abstract bool PostPerform();
    }
}
