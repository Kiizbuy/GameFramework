using GameFramework.Events;
using UnityEngine;
using Zenject;
using GameFramework.CustomCollections;

namespace GameFramework.Debugging
{
    public class ZenjectBindTest : MonoBehaviour
    {
        [SerializeField] public SerializableDictionaryBase<string, Vector3> _huyDictionary = new SerializableDictionaryBase<string, Vector3>();

        //[Inject]
        //private GlobalEventsRouter globalEventsRouter;

        void Start()
        {
            //Debug.Log(globalEventsRouter.gameObject.name);
        }
    }
}

