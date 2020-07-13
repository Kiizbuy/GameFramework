using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Components
{
    public class OnKeyboardGameObjectActiveToggle : MonoBehaviour
    {
        [SerializeField] private GameObject toggleGameObject;
        [SerializeField] private bool defaultActiveState = false;
        [SerializeField] private KeyCode keyCodeToggle = KeyCode.F1;

        private void Start()
        {
            if (toggleGameObject == null)
            {
                toggleGameObject = gameObject;
                toggleGameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(keyCodeToggle))
                toggleGameObject.SetActive(!toggleGameObject.activeSelf);
        }
    }
}

