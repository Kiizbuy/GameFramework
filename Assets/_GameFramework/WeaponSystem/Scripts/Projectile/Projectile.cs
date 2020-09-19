using GameFramework.Components;
using System;
using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public enum PhysicInteractionType
    {
        Collider,
        Trigger
    };

    public struct ProjectileDataInfo
    {
        public readonly PhysicInteractionType PhysicInteractionType;
        public readonly Vector3 Direction;
        public readonly float Force;
        public readonly int Damage;

        public ProjectileDataInfo(PhysicInteractionType physicInteractionType, Vector3 direction, float force, int damage)
        {
            PhysicInteractionType = physicInteractionType;
            Direction = direction;
            Force = force;
            Damage = damage;
        }
    }

    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour, IWeapon, IAttackable
    {
        public event Func<IHealth> OnTargetHasDamaged;
        public event Action OnProjectileLiveEnd;

        private Rigidbody _rigidbody;
        private PhysicInteractionType _physicInteractionType;

        public int Damage { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void PushProjectile(ProjectileDataInfo projectileInfo, Action projectileDeadAction, Func<IHealth> targetHasDamagedAction)
        {
            Damage = projectileInfo.Damage;
            _physicInteractionType = projectileInfo.PhysicInteractionType;
            _rigidbody.AddForce(projectileInfo.Direction * projectileInfo.Force, ForceMode.Impulse);

            OnTargetHasDamaged += targetHasDamagedAction;
            OnProjectileLiveEnd += projectileDeadAction;
        }


        public void Attack()
        {
            return;
        }

        public bool CanAttack() => true;

    }

}

