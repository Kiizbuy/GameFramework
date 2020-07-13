using GameFramework.Events;
using System;
using UnityEngine;

[System.Serializable]
public class PidarasClass
{
    public event Action<EventParameter> OnSuck;
    [EventName(nameof(OnSuck))]
    public EventToMethodSubscribeСontainer OnSuckSubscriber = new EventToMethodSubscribeСontainer();

    public Vector3 VecValue;

    public PidarasClass(Vector3 vecValue)
    {
        VecValue = vecValue;
    }
}

[System.Serializable]
public class NestedClassEventTest
{
    public event Action<EventParameter> OnCall;
    [EventName(nameof(OnCall))]
    public EventToMethodSubscribeСontainer OnCallSubscriber = new EventToMethodSubscribeСontainer();

    public PidarasClass pidarasClass;

    public void InvokeEvent() => OnCall?.Invoke(null);

    public void Subscribe() => EventSubscriber.Subscribe(this);
}

public class CustomEventUsageExample : MonoBehaviour
{
    public NestedClassEventTest t;

    public event Action<EventParameter> OnSyka;
    public event Action<EventParameter> OnBlya;

    [MethodName(nameof(HUITA))]
    public MethodToEventSubscribeContainer HUITASubscriber = new MethodToEventSubscribeContainer();

    [EventName(nameof(OnSyka))]
    public EventToMethodSubscribeСontainer OnSykaEventSubscriber = new EventToMethodSubscribeСontainer();
    [EventName(nameof(OnBlya))]
    public EventToMethodSubscribeСontainer OnBlyaEventSubscriber = new EventToMethodSubscribeСontainer();

    private void Start()
    {
        EventSubscriber.Subscribe(this);
        t.Subscribe();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            var eventParameter = new EventParameter();

            eventParameter.TryAddCustomDataParameter("Vec", new PidarasClass(Vector3.one));

            OnSyka?.Invoke(eventParameter);
        }

        if (Input.GetKeyDown(KeyCode.B))
            OnBlya?.Invoke(new EventParameter_GameObject(gameObject));

        if (Input.GetKeyDown(KeyCode.C))
            t.InvokeEvent();
    }

    public void HUITA(EventParameter h)
    {
        if(h != null && h.TryGetCustomDataParameterValue<PidarasClass>("Vec", out var pidaras))
            Debug.Log(pidaras.VecValue);

        Debug.Log("Idi nahuy");
    }
}
