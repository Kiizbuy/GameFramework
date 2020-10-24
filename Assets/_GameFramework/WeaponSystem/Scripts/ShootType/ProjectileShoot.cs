using GameFramework.Components;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework.WeaponSystem
{
    public class ProjectileShoot : IShootType
    {
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private Projectile _projectileModel;
        [SerializeField] private PhysicInteractionType _physicInteractionType;
        [SerializeField] private float _shootForce = 5f;

        public void TryTakeDamageOnTarget(int Damage, IAttackable attackable)
        {
            var projectileParameters = new ProjectileDataInfo(_physicInteractionType, _projectileSpawnPoint.TransformDirection(_projectileSpawnPoint.forward), _shootForce, Damage);
            var projectile = Object.Instantiate(_projectileModel, _projectileSpawnPoint.position, _projectileSpawnPoint.rotation);

            projectile.PushProjectile(projectileParameters);
        }
    }
}
