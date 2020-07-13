using System;
using GameFramework.Events;
using UnityEngine;

namespace GameFramework.Components
{
    public class Physics2DCollisionCallbackInvoker : MonoBehaviour
    {
        public event Action<EventParameter> OnCollisionEntered;
        public event Action<EventParameter> OnCollisionStayed;
        public event Action<EventParameter> OnCollisionGone;

        [EventName(nameof(OnCollisionEntered))]
        public MethodToEventSubscribeContainer OnCollisionEnterSubscriber = new MethodToEventSubscribeContainer();
        [EventName(nameof(OnCollisionStayed))]
        public MethodToEventSubscribeContainer OnCollisionStaySubscriber = new MethodToEventSubscribeContainer();
        [EventName(nameof(OnCollisionGone))]
        public MethodToEventSubscribeContainer OnCollisionExitSubscriber = new MethodToEventSubscribeContainer();

        public bool UseOnCollisionEnter = true;
        public bool UseOnCollisionStay = false;
        public bool UseOnCollisionExit = false;

        [SerializeField] private string _objectTag;

        private void Start()
        {
            EventSubscriber.Subscribe(this);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!UseOnCollisionEnter)
                return;

            var taggableObject = collision.collider.GetComponent<TaggableObject>();

            if (taggableObject != null && taggableObject.HaveTag(_objectTag))
                OnCollisionEntered?.Invoke(new EventParameter_Collision2D(collision));
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!UseOnCollisionStay)
                return;

            var taggableObject = collision.collider.GetComponent<TaggableObject>();

            if (taggableObject != null && taggableObject.HaveTag(_objectTag))
                OnCollisionEntered?.Invoke(new EventParameter_Collision2D(collision));
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!UseOnCollisionExit)
                return;
            
            var taggableObject = collision.collider.GetComponent<TaggableObject>();

            if (taggableObject != null && taggableObject.HaveTag(_objectTag))
                OnCollisionGone?.Invoke(new EventParameter_Collision2D(collision));
        }
    }
}
