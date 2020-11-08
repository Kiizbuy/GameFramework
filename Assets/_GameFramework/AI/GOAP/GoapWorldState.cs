using System;
using System.Collections.Generic;

namespace GameFramework.AI.GOAP
{

    [Serializable]
    public class WorldState
    {
        public string Key;
        public int Value;
    }

    public class GoapWorldState
    {
        private readonly Dictionary<string, int> _worldStates;

        public GoapWorldState()
        {
            _worldStates = new Dictionary<string, int>();
        }

        public IReadOnlyDictionary<string, int> GetWorldStates => _worldStates;

        public bool HasState(string keyState)
        {
            return _worldStates.ContainsKey(keyState);
        }

        public void AddState(string keyState, int valueState)
        {
            if (!_worldStates.ContainsKey(keyState))
                _worldStates.Add(keyState, valueState);
        }

        public void ModifyState(string keyState, int newValueState)
        {
            if (_worldStates.ContainsKey(keyState))
            {
                _worldStates[keyState] += newValueState;
                if (_worldStates[keyState] <= 0)
                    RemoveState(keyState);
            }
            else
            {
                _worldStates.Add(keyState, newValueState);
            }
        }

        public void SetState(string keyState, int valueState)
        {
            _worldStates[keyState] = valueState;
        }

        public void RemoveState(string keyState)
        {
            if (_worldStates.ContainsKey(keyState))
                _worldStates.Remove(keyState);
        }
    }
}
