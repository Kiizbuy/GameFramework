using UnityEngine;
using GameFramework.Strategy;
using GameFramework.Components;
using UnityEngine.Events;
using GameFramework.UnityEngine.EventsExtension;
using NaughtyAttributes;
using System.Collections;

namespace GameFramework.WeaponSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class GunWeapon : MonoBehaviour, IWeapon, IAttackable
    {
        [BoxGroup("Events")]
        public UnityEventInt OnShoot;
        [BoxGroup("Events")]
        public UnityEventFloat OnReloading;
        [BoxGroup("Events")]
        public UnityEvent OnShootHasEmpty;

        [SerializeReference, StrategyContainer]
        public IShootType ShootType;
        
        [SerializeField] [BoxGroup("Visual Settings")]
        private AudioClip _shootSound;
        [SerializeField] [BoxGroup("Visual Settings")]
        private AudioClip _emptyShootSound;
        [SerializeField] [BoxGroup("Visual Settings")]
        private ParticleSystem _muzzleFlash;

        [SerializeField] [BoxGroup("WeaponData")]
        private GunWeaponData _currentGunWeaponDataInfo;

        [SerializeField] [BoxGroup("info")]
        private int _currentAmmoClipCount = 120;
        [SerializeField] [BoxGroup("info")]
        private int _currentAmmoCount = 120;


        public int Damage => _currentGunWeaponDataInfo.Damage;
        public bool CanReload => (_currentAmmoCount < _currentAmmoClipCount && _currentGunWeaponDataInfo.MaxAmmoReloadCount > 0) && _isReloading == false;
        public bool CanAttack() => _currentAmmoCount > 0 && _isReloading == false;

        private AudioSource _audioSource;
        private bool _isReloading;
        private float _currentReloadTimer;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void ChangeWeaponDataInfo(GunWeaponData value) => _currentGunWeaponDataInfo = value;

        public void Attack()
        {
            if (CanAttack())
            {
                _currentAmmoCount--;
                _audioSource.PlayOneShot(_shootSound);
                _muzzleFlash?.Play();

                if (ShootType.HitInTarget(out var healthTarget))
                    healthTarget.TakeDamage(Damage, this);
            }
            else
            {
                _audioSource.PlayOneShot(_emptyShootSound);
            }
        }

        public void ReloadWeapon()
        {
            if (!CanReload)
                return;

            while(_currentReloadTimer < _currentGunWeaponDataInfo.ReloadTime)
            {
                _currentReloadTimer += Time.deltaTime;
                OnReloading?.Invoke(_currentReloadTimer);

                if(_currentReloadTimer >= _currentGunWeaponDataInfo.ReloadTime)
                {

                }
            }
        }

        //private IEnumerator ReloadCoroutine()
        //{
        //    var currentReloadingTime = 0f;
        //    var currentReloadAmmoValue = currentReloadAmmoCount - ammoPerClip;

        //    if (currentReloadAmmoCount > currentReloadAmmoValue)
        //    {
        //        var ammoToLoad = currentAmmoCount - ammoPerClip;
        //        currentReloadAmmoCount -= currentReloadAmmoValue;
        //        currentAmmoCount = currentReloadAmmoValue;
        //    }
        //    else
        //    {
        //        canShoot = false;
        //        OnCantReload?.Invoke();
        //        yield break;
        //    }

        //    while (currentReloadingTime < reloadDelay)
        //    {
        //        yield return new WaitForFixedUpdate();
        //        var normalizedReloadTime = currentReloadingTime / reloadDelay;

        //        Debug.Log(normalizedReloadTime);
        //        currentReloadingTime += Time.deltaTime;
        //        OnReloading?.Invoke(currentReloadingTime);
        //    }

        //    canShoot = true;
        //}
    }
}
