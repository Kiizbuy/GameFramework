using System;
using GameFramework.Events;
using UnityEngine;

namespace GameFramework.Components
{
    public class Physics2DTriggerCallbackInvoker : MonoBehaviour
    {
        public event Action<EventParameter> OnTriggerEntered;
        public event Action<EventParameter> OnTriggerStayed;
        public event Action<EventParameter> OnTriggerGone;

        [EventName(nameof(OnTriggerEntered))]
        public MethodToEventSubscribeContainer OnTriggerEnterSubscriber = new MethodToEventSubscribeContainer();
        [EventName(nameof(OnTriggerStayed))]
        public MethodToEventSubscribeContainer OnTriggerStaySubscriber = new MethodToEventSubscribeContainer();
        [EventName(nameof(OnTriggerGone))]
        public MethodToEventSubscribeContainer OnTriggerExitSubscriber = new MethodToEventSubscribeContainer();

        public bool UseOnTriggerEnter = true;
        public bool UseOnTriggerStay = false;
        public bool UseOnTriggerExit = false;

        [SerializeField] private string _objectTag;

        private void Start()
        {
            EventSubscriber.Subscribe(this);
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!UseOnTriggerEnter)
                return;

            var taggableObject = collision.GetComponent<TaggableObject>();

            if (taggableObject != null && taggableObject.HaveTag(_objectTag))
                OnTriggerEntered?.Invoke(new EventParameter_Collider2D(collision));
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if(!UseOnTriggerStay)
                return;

            var taggableObject = collision.GetComponent<TaggableObject>();

            if (taggableObject != null && taggableObject.HaveTag(_objectTag))
                OnTriggerStayed?.Invoke(new EventParameter_Collider2D(collision));
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!UseOnTriggerExit)
                return;

            var taggableObject = collision.GetComponent<TaggableObject>();

            if (taggableObject != null && taggableObject.HaveTag(_objectTag))
                OnTriggerGone?.Invoke(new EventParameter_Collider2D(collision));
        }
    }
}
