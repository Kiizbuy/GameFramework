using GameFramework.Components;
using System;
using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public struct ProjectileInfo
    {
        public readonly Vector3 Direction;
        public float Force;

        public ProjectileInfo(Vector3 direction, float force)
        {
            Direction = direction;
            Force = force;
        }
    }

    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        public event Func<IHealth> OnTargetHasDamaged;
        public event Action OnProjectileLiveEnd;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void PushProjectile(ProjectileInfo projectileInfo, Action projectileDeadAction, Func<IHealth> targetHasDamagedAction)
        {
            _rigidbody.AddForce(projectileInfo.Direction * projectileInfo.Force, ForceMode.Impulse);
            OnTargetHasDamaged += targetHasDamagedAction;
            OnProjectileLiveEnd += projectileDeadAction;
        }
    }

}

