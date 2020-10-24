using NaughtyAttributes;
using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public enum FireType
    {
        Semi,
        Automatic
    }

    [CreateAssetMenu(menuName = "Weapon/Gun/Create weapon data", fileName = "GunWeaponData")]
    public class GunWeaponData : ScriptableObject
    {
        [SerializeField] private FireType _fireType;
        [SerializeField] private float _fireDelay;
        [SerializeField] private float _reloadTime;
        [SerializeField] private int _maxAmmoCount;
        [SerializeField] private int _reloadAmmoCount;
        [SerializeField, MinMaxSlider(0f, 350f)] private Vector2 _minMaxDamage;

        public FireType FireType;
        public float FireDelay;
        public float ReloadTime;
        public int MaxAmmoReloadCount;
        public int ReloadAmmoCount;
        public int Damage => (int)Random.Range(_minMaxDamage.x, _minMaxDamage.y);
    }
}
