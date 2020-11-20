using DG.Tweening;
using System;
using UnityEngine;

namespace GameFramework.InteractableSystem
{
    public enum DoorState
    {
        Open = 0,
        Closed = 1,
        Locked = 2
    }

    public enum OpenType
    {
        Move,
        Rotate,
        Both
    }

    public class Door : MonoBehaviour, IInteractable
    {
        [Range(0, 1)]
        public float previewPosition;

        [SerializeField] private DoorState _doorState;
        [SerializeField] private OpenType _openType;
        [SerializeField] private Transform _doorPoint;

        [SerializeField] private float _doorSpeed = 5f;

        //TODO - Move to SO settings
        [SerializeField] private Vector3 _startOpenPoint;
        [SerializeField] private Vector3 _endOpenPoint;
        [SerializeField] private Vector3 _startOpenRotation;
        [SerializeField] private Vector3 _endOpenRotation;

        public void LockDoor() => _doorState = DoorState.Locked;
        public void UnlockDoor() => _doorState = DoorState.Closed;

        public bool InteractRequirements()
        {
            return _doorState != DoorState.Locked;
        }

        public void Interact()
        {
            if (_doorState == DoorState.Locked)
                return;
            _doorState = _doorState == DoorState.Open ? DoorState.Closed : DoorState.Open;

            switch (_doorState)
            {
                case DoorState.Open:
                    MoveDoor(_endOpenPoint, _endOpenRotation);
                    break;
                case DoorState.Closed:
                    MoveDoor(_startOpenPoint, _startOpenRotation);
                    break;
                case DoorState.Locked:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void MoveDoor(Vector3 endPosition, Vector3 endRotation)
        {
            switch (_openType)
            {
                case OpenType.Move:
                    {

                    }
                    //var endPos = _doorPoint.TransformPoint(_doorPoint.position - endPosition);
                    break;
                case OpenType.Rotate:
                    _doorPoint.DORotate(_doorPoint.TransformPoint(endRotation), _doorSpeed);
                    break;
                case OpenType.Both:
                    _doorPoint.DOMove(_doorPoint.TransformPoint(endPosition), _doorSpeed);
                    _doorPoint.DORotate(_doorPoint.TransformPoint(endRotation), _doorSpeed);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
