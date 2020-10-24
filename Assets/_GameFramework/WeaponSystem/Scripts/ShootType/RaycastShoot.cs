using GameFramework.Components;
using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public class RaycastShoot : IShootType
    {
        [SerializeField] private Transform _raycastOrigin;
        [SerializeField] private LayerMask _shootMask = ~0;
        [SerializeField] private float _shootRange = 3f;
        [SerializeField] private ProjectileViewMover _projectileViewMover;


        public void TryTakeDamageOnTarget(int damage, IAttackable attackable)
        {
            if (Physics.Raycast(_raycastOrigin.position, _raycastOrigin.forward, out var hit, _shootRange, _shootMask, QueryTriggerInteraction.Collide))
            {
                var projectileViewInfo = new ProjectileViewMoverInfo(hit.distance, 5f);
                var projectileMover = Object.Instantiate(_projectileViewMover, _raycastOrigin.position,
                    _raycastOrigin.rotation);

                projectileMover.PushProjectileView(projectileViewInfo);

                if (hit.collider.TryGetComponent(out IHealth healthComponent))
                    healthComponent.TakeDamage(damage, attackable);
            }
        }
    }
}

