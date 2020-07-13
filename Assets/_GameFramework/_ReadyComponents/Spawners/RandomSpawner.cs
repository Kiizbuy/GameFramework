using UnityEngine;
using System;
using GameFramework.Events;
using GameFramework.Extension;
using NaughtyAttributes;

namespace GameFramework.Components.Spawners
{
    public class RandomSpawner : MonoBehaviour, ISpawner
    {
        public event Action<EventParameter> OnSpawned;
        [EventName(nameof(OnSpawned))]
        public EventToMethodSubscribeСontainer OnSpawnedSubscriber = new EventToMethodSubscribeСontainer();
        [MethodName(nameof(TriggerSpawn))]
        public MethodToEventSubscribeContainer TriggerSpawnSubscriber = new MethodToEventSubscribeContainer();

        [SerializeField, ReorderableList]
        private GameObject[] _spawnableObjects = new GameObject[0];
        [SerializeField, ReorderableList]
        private Transform[] _spawnPoints = new Transform[0];

        private void Start()
        {
            EventSubscriber.Subscribe(this);
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (_spawnPoints.Length > 0)
            {
                Gizmos.color = Color.green;

                foreach (var spawnPoint in _spawnPoints)
                    Gizmos.DrawWireSphere(spawnPoint.position, 1f);
            }

        }
#endif

        public void SpawnObject(SpawnableObjectInfo spawnableObjectInfo, Transform spawnParent = null)
        {
            var instantiatedObject = Instantiate(spawnableObjectInfo.SpawnableObject,
                                                 spawnableObjectInfo.SpawnPoint,
                                                 spawnableObjectInfo.SpawnRotation,
                                                 spawnParent);

            OnSpawned?.Invoke(new EventParameter_GameObject(instantiatedObject));
        }

        public void TriggerSpawn(EventParameter param)
        {
            if (_spawnableObjects.Length == 0)
            {
                Debug.LogError("SpawnableObjects Array is Empty", gameObject);
                return;
            }

            if (_spawnPoints.Length == 0)
            {
                Debug.LogError("SpawnPoints Array is Empty", gameObject);
                return;
            }

            var randomObject = _spawnableObjects.GetRandomElement();
            var randomSpawnPoint = _spawnPoints.GetRandomElement();

            SpawnObject(new SpawnableObjectInfo(randomObject, randomSpawnPoint.position, randomSpawnPoint.rotation));
        }
    }

}

