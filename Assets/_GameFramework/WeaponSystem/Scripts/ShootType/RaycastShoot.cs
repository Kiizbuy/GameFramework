using GameFramework.Components;
using NaughtyAttributes;
using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public class RaycastShoot : IShootType
    {
        [SerializeField, Required] private Transform _raycastOrigin;
        [SerializeField] private LayerMask _shootMask = ~0;
        [SerializeField] private float _shootRange = 3f;
        [SerializeField] private ProjectileViewMover _projectileViewMover;
        [SerializeField] private float _minMaxConeProjectileRotationOffset = 5f;

        private const float _maxProjectileLiveDistance = 400f;
        private const float _maxProjectileFlySpeed = 155f;

        public void ShootAndTryTakeDamage(int damage, IAttackable attackable)
        {
            if (Physics.Raycast(_raycastOrigin.position, _raycastOrigin.forward, out var hit, _shootRange, _shootMask,
                QueryTriggerInteraction.Collide))
            {
                if (hit.collider.TryGetComponent(out IHealth healthComponent))
                    healthComponent.TakeDamage(damage, attackable);
            }

            if (_projectileViewMover == null)
                return;
            var rotationAdditional = Quaternion.Euler(Random.Range(_minMaxConeProjectileRotationOffset, _minMaxConeProjectileRotationOffset),
                                                              Random.Range(-_minMaxConeProjectileRotationOffset, _minMaxConeProjectileRotationOffset),
                                                              0);

            var projectileViewInfo = new ProjectileViewMoverInfo((hit.collider != null ? hit.distance : _maxProjectileLiveDistance), _maxProjectileFlySpeed);
            var projectileMover = Object.Instantiate(_projectileViewMover, _raycastOrigin.position,
                _raycastOrigin.rotation * rotationAdditional);

            projectileMover.PushProjectileView(projectileViewInfo);
        }

        public void DrawGizmos()
        {
            if (_raycastOrigin == null)
                return;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_raycastOrigin.position, _raycastOrigin.position + _raycastOrigin.forward * _shootRange);
        }
    }
}
