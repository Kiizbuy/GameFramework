using GameFramework.Components;
using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public class ProjectileShoot : IShootType
    {
        [SerializeField] private GameObject _projectileModel;


        public bool HitInTarget(out IHealth healthTarget)
        {
            throw new System.NotImplementedException();
        }
    }
}
