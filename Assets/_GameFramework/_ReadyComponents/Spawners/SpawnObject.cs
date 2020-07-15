using System;
using UnityEngine;
using GameFramework.Events;
using GameFramework.Strategy;
using GameFramework.Extension;

using System.Collections.Generic;
using NaughtyAttributes;

namespace GameFramework.Components
{
    public interface ISpawner : IStrategyContainer
    {
        event Action<EventParameter> OnSpawn;
        void Spawn(EventParameter parameter);
        void Subscribe();
    }

    public interface IObjectPlacer : IStrategyContainer
    {
        event Action<EventParameter> OnHandled;
        void Handle(EventParameter parameter);
        void Place(EventParameter parameter);
        void Subscribe();
    }

    public class SingleSpawnObject : ISpawner
    {
        public event Action<EventParameter> OnSpawn;

        [SerializeField] private GameObject _spawnableObject;


        public void Spawn(EventParameter parameter)
        {
            var spawnedObject = GameObject.Instantiate(_spawnableObject);

            OnSpawn?.Invoke(new EventParameter_GameObject(spawnedObject));
        }

        public void Subscribe()
        {
            throw new NotImplementedException();
        }
    }

    public class RandomSpawnObject : ISpawner
    {
        public event Action<EventParameter> OnSpawn;

        [SerializeField] private List<GameObject> _randomObjects = new List<GameObject>();

        public void Handle(EventParameter parameter)
        {
            throw new NotImplementedException();
        }

        public void Spawn(EventParameter parameter)
        {
            var randomObject = GameObject.Instantiate( _randomObjects.GetRandomElement());

            OnSpawn?.Invoke(new EventParameter_GameObject(randomObject));
        }

        public void Subscribe()
        {
            throw new NotImplementedException();
        }
    }

    public class PlaceOnTransformPosition : IObjectPlacer
    {
        public event Action<EventParameter> OnHandled;

        [MethodName(nameof(Handle))]
        public MethodToEventSubscribeContainer HandleSubscriber = new MethodToEventSubscribeContainer();

        [SerializeField] private Transform _transformPoint;

        public void Handle(EventParameter parameter)
        {
            throw new NotImplementedException();
        }

        public void Place(EventParameter parameter)
        {
            throw new NotImplementedException();
        }

        public void Subscribe()
        {
            throw new NotImplementedException();
        }
    }

    public class SpawnObject : MonoBehaviour
    {
        public event Action<EventParameter> OnSpawn;

        [EventName(nameof(OnSpawn))]
        public EventToMethodSubscribeСontainer OnSpawnSubscriber = new EventToMethodSubscribeСontainer();
        [MethodName(nameof(SpawnTrigger))]
        public MethodToEventSubscribeContainer SpawnTriggerSubscriber = new MethodToEventSubscribeContainer();

        [SerializeReference, StrategyContainer]
        public ISpawner SpawnerType;
        [SerializeReference, StrategyContainer]
        public IObjectPlacer ObjectPlaceType;


        private void Start()
        {
            if (SpawnerType == null)
            {
                Debug.LogError("Spawner type is null, please, assign him", this);
                return;
            }

            if (ObjectPlaceType == null)
            {
                Debug.LogError("Object Place Type is null, please, assign him", this);
                return;
            }


            SpawnerType.OnSpawn += Spawned;
            SpawnerType.Subscribe();
            ObjectPlaceType.OnHandled += (p) => OnSpawn?.Invoke(p);

            ObjectPlaceType.Subscribe();

            EventSubscriber.Subscribe(this);
        }

        public void SpawnTrigger(EventParameter parameter)
        {
            SpawnerType.Spawn(parameter);
        }

        private void Spawned(EventParameter parameter)
        {
            if (ObjectPlaceType != null)
                ObjectPlaceType.Handle(parameter);
            else
                OnSpawn?.Invoke(parameter);
        }
    }

}

