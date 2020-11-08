using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.AI.GOAP
{
    public abstract class GoapAction : MonoBehaviour
    {
        public string ActionName = "ActionName";
        public float ActionCost = 4f;
        public GameObject Target;
        public GameObject TargetTag;
        public float Duration;
        public WorldState[] PreConditions;
        public WorldState[] AfterEffects;
        public WorldState AgentBeliefs;
        public bool Running;

        public Dictionary<string, int> Preconditions = new Dictionary<string, int>();
        public Dictionary<string, int> Aftereffects = new Dictionary<string, int>();

        public bool IsAchievable => true;

        public bool IsAchievableHasGiven(IReadOnlyDictionary<string, int> conditions)
        {
            foreach (var p in Preconditions)
            {
                if (!Preconditions.ContainsKey(p.Key))
                    return false;
            }

            return true;
        }

        public abstract bool PrePerform();
        public abstract bool PostPerform();
    }
}
