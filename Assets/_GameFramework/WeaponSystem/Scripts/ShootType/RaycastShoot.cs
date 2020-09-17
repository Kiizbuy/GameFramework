using GameFramework.Components;
using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public class RaycastShoot : IShootType
    {
        [SerializeField] private Transform _raycastOrigin;
        [SerializeField] private LayerMask _shootMask = ~0;
        [SerializeField] private float _shootRange = 3f;

        public bool HitInTarget(out IHealth healthTarget)
        {
            // Пример плохой спроектированной системы рейкаста - ебал я в рот столько параметров
            if(Physics.Raycast(_raycastOrigin.position, _raycastOrigin.forward, out var hit, _shootRange ,_shootMask, QueryTriggerInteraction.Collide))
            {
                if(hit.collider.TryGetComponent(out IHealth healthComponent))
                {
                    healthTarget = healthComponent;
                    return true;
                }
            }

            healthTarget = default;
            return false;
        }
    }
}

