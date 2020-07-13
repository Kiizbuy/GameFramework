using UnityEngine;
using System;
using GameFramework.Events;
using NaughtyAttributes;

namespace GameFramework.Components.Spawners
{
    public interface ISpawner
    {
        event Action<EventParameter> OnSpawned;
        void SpawnObject(SpawnableObjectInfo spawnableObjectInfo, Transform spawnParent = null);
        void TriggerSpawn(EventParameter param);
    }

    public struct SpawnableObjectInfo
    {
        public readonly GameObject SpawnableObject;
        public readonly Vector3 SpawnPoint;
        public readonly Quaternion SpawnRotation;

        public SpawnableObjectInfo(GameObject spawnableObject, Vector3 spawnPoint, Quaternion spawnRotation)
        {
            SpawnableObject = spawnableObject;
            SpawnPoint = spawnPoint;
            SpawnRotation = spawnRotation;
        }
    }

    public class SimpleSpawner : MonoBehaviour, ISpawner
    {
        public event Action<EventParameter> OnSpawned;
        [EventName(nameof(OnSpawned))]
        public EventToMethodSubscribeСontainer OnSpawnedSubscriber = new EventToMethodSubscribeСontainer();
        [MethodName(nameof(TriggerSpawn))]
        public MethodToEventSubscribeContainer TriggerSpawnSubscriber = new MethodToEventSubscribeContainer();

        [SerializeField, Required]
        private GameObject _spawbableObject;
        [SerializeField, Required]
        private Transform _spawnPoint;

        private void Start()
        {
            EventSubscriber.Subscribe(this);
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (_spawnPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_spawnPoint.position, 1f);
            }

        }
#endif
        public void TriggerSpawn(EventParameter param)
        {
            SpawnObject(new SpawnableObjectInfo(_spawbableObject, _spawnPoint.position, _spawnPoint.rotation));
        }

        public void SpawnObject(SpawnableObjectInfo spawnableObjectInfo, Transform spawnParent = null)
        {
            var instantiatedObject = Instantiate(spawnableObjectInfo.SpawnableObject, 
                                                 spawnableObjectInfo.SpawnPoint, 
                                                 spawnableObjectInfo.SpawnRotation, 
                                                 spawnParent);

            OnSpawned?.Invoke(new EventParameter_GameObject(instantiatedObject));
        }
    }

}

