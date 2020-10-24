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
        [SerializeField] private int _maxAmmoCapacity;
        [SerializeField] private int _reloadAmmoPerClipCapacity;
        [SerializeField, MinMaxSlider(0f, 350f)] private Vector2 _minMaxDamage;

        public FireType FireType => _fireType;
        public float FireDelay => _fireDelay;
        public float ReloadTime => _reloadTime;
        public int MaxAmmoCapacity => _maxAmmoCapacity;
        public int ReloadAmmoPerClipCapacity => _reloadAmmoPerClipCapacity;
        public int Damage => (int)Random.Range(_minMaxDamage.x, _minMaxDamage.y);
    }
}
