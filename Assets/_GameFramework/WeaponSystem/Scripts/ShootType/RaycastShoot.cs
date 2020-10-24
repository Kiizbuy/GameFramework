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

        public bool TryShoot(int damage, IAttackable attackable)
        {
            if (!Physics.Raycast(_raycastOrigin.position, _raycastOrigin.forward, out var hit, _shootRange, _shootMask,
                QueryTriggerInteraction.Collide))
                return false;

            var projectileViewInfo = new ProjectileViewMoverInfo(hit.distance, 5f);

            if (_projectileViewMover != null)
            {
                var projectileMover = Object.Instantiate(_projectileViewMover, _raycastOrigin.position,
                    _raycastOrigin.rotation);

                if (projectileMover)
                    projectileMover.PushProjectileView(projectileViewInfo);
            }

            if (hit.collider.TryGetComponent(out IHealth healthComponent))
                healthComponent.TakeDamage(damage, attackable);

            return true;

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
