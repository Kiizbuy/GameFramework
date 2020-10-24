using GameFramework.Components;
using GameFramework.Strategy;
using GameFramework.UnityEngine.EventsExtension;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameFramework.WeaponSystem
{
    public readonly struct AmmoInfoState
    {
        public readonly int AmmoCount;
        public readonly int AmmoClipCapacity;

        public AmmoInfoState(int ammoCount, int ammoClipCapacity)
        {
            AmmoCount = ammoCount;
            AmmoClipCapacity = ammoClipCapacity;
        }
    }

    [Serializable]
    public class AmmoChangedEvent : UnityEvent<AmmoInfoState> { }

    [RequireComponent(typeof(AudioSource))]
    public class GunWeapon : MonoBehaviour, IWeapon, IAttackable
    {
        [BoxGroup("UI Events")]
        public AmmoChangedEvent OnAmmoCountHasChanged;
        [BoxGroup("UI Events")]
        public UnityEventFloat OnReloading;
        [BoxGroup("UI Events")]
        public UnityEvent OnShootHasEmpty;

        [SerializeReference, StrategyContainer]
        public IShootType ShootType;

        [SerializeField]
        [BoxGroup("Visual Settings")]
        private AudioClip _shootSound;
        [SerializeField]
        [BoxGroup("Visual Settings")]
        private AudioClip _emptyShootSound;
        [SerializeField]
        [BoxGroup("Visual Settings")]
        private ParticleSystem _muzzleFlash;

        [SerializeField]
        [BoxGroup("WeaponData")]
        private GunWeaponData _gunWeaponData;

        [SerializeField]
        [BoxGroup("Info")]
        private int _ammoClipCapacity = 120;
        [SerializeField]
        [BoxGroup("Info")]
        private int _ammoCount = 120;

        public int Damage => _gunWeaponData.Damage;
        public bool CanReload => _isReloading == false && (_ammoCount < _ammoClipCapacity && _ammoClipCapacity > 0);
        public bool CanAttack() => _ammoCount > 0 && _isReloading == false;

        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

        private AudioSource _audioSource;
        private bool _isReloading;
        private float _currentReloadTimer;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnDrawGizmos()
        {
            var gizmos = ShootType as IStrategyDrawGizmos;
            if (ShootType != null)
                gizmos?.DrawGizmos();
        }

        public void ChangeWeaponDataInfo(GunWeaponData value)
        {
            _gunWeaponData = value;
            _ammoClipCapacity = value.ReloadAmmoPerClipCapacity;
        }

        public void AddClipAmmoCapacity(int ammo)
        {
            _ammoClipCapacity = Mathf.Clamp(_ammoClipCapacity + ammo, 0, _gunWeaponData.MaxAmmoCapacity);
        }

        public void Attack()
        {
            if (CanAttack())
            {
                if (!(_shootSound is null))
                    _audioSource.PlayOneShot(_shootSound);

                if (_muzzleFlash != null)
                    _muzzleFlash.Play();

                if (!ShootType.TryShoot(Damage, this))
                    return;

                _ammoCount--;
                OnAmmoCountHasChanged?.Invoke(new AmmoInfoState(_ammoCount, _ammoClipCapacity));

                Debug.Log("Fire");
            }
            else
            {
                if (!(_emptyShootSound is null))
                    _audioSource.PlayOneShot(_emptyShootSound);

                OnShootHasEmpty?.Invoke();
            }
        }

        public void ReloadWeapon()
        {
            StartCoroutine(ReloadCoroutine());
        }

        private IEnumerator ReloadCoroutine()
        {
            if (!CanReload)
                yield break;

            _isReloading = true;
            _currentReloadTimer = 0f;
            Debug.Log("Begin reload");

            while (_currentReloadTimer < _gunWeaponData.ReloadTime)
            {
                yield return _waitForFixedUpdate;
                _currentReloadTimer += Time.deltaTime;
                OnReloading?.Invoke(_currentReloadTimer);

                Debug.Log($"Reload timer {_currentReloadTimer}");

                if (_currentReloadTimer >= _gunWeaponData.ReloadTime)
                {
                    var countToReload = _gunWeaponData.ReloadAmmoPerClipCapacity - _ammoCount;

                    if (_ammoClipCapacity < countToReload)
                    {
                        _ammoCount = countToReload;
                        _ammoClipCapacity = 0;
                    }
                    else
                    {
                        _ammoCount += countToReload;
                        _ammoClipCapacity -= countToReload;
                    }

                    _isReloading = false;
                    Debug.Log("Reload has been ended");
                    OnAmmoCountHasChanged?.Invoke(new AmmoInfoState(_ammoCount, _ammoClipCapacity));
                }
            }
        }
    }
}
