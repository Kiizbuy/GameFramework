using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.AI.SimpleBehaviorTree
{
    public class AIBrainStorage : MonoBehaviour
    {
        [SerializeField] private List<AIBehaviorAdapter> _behaviors;
    }

    [System.Serializable]
    public class AIBehaviorAdapter
    {
        [SerializeField] private float _baseBehaviorPriority;
        [SerializeField] private AnimationCurve _behaviorPriorityEvaluateCurve;
    }
}
