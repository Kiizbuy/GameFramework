using GameFramework.Components;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFramework.Events
{
    public class GlobalEventsRouter : SingletonBehaviour<GlobalEventsRouter>
    {
        private Dictionary<string, Action<EventParameter>> eventDictionary;

        protected override bool OrderDontDestroyOnLoad => true;

        protected override void OnAwake()
            => Init();

        protected override void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
            => Init();

        private void Init()
        {
            if (!HasInstance)
                instance = GetOrCreateInstance;
            else
                eventDictionary = new Dictionary<string, Action<EventParameter>>();
        }

        public static void StartListeningGlobalEvent(string eventName, Action<EventParameter> listener)
        {
            if (!HasInstance)
            {
                instance = GetOrCreateInstance;
                if (!HasInstance)
                {
                    Debug.LogError("No instance of GlobalEventsRouter");
                    return;
                }
            }

            if (instance.eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent += listener;
                instance.eventDictionary[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                instance.eventDictionary.Add(eventName, thisEvent);
            }

            Debug.Log($"GLOBAL EVENT:{eventName} is listening");
        }

        public static void StopListeningGlobalEvent(string eventName, Action<EventParameter> listener)
        {
            if (!HasInstance)
            {
                instance = GetOrCreateInstance;
                if (!HasInstance)
                {
                    Debug.LogError("No instance of GlobalEventsRouter");
                    return;
                }
            }

            if (instance.eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent -= listener;
                instance.eventDictionary[eventName] = thisEvent;
            }
        }

        public static void RaiseGlobalEvent(string eventName, EventParameter param)
        {
            if (!HasInstance)
            {
                instance = GetOrCreateInstance;
                if (!HasInstance)
                {
                    Debug.LogError("No instance of GlobalEventsRouter");
                    return;
                }
            }

            Debug.Log($"RAISE GLOBAL EVENT: {eventName}");

            if (instance.eventDictionary.TryGetValue(eventName, out var thisEvent))
                thisEvent.Invoke(param);
        }
    }
}