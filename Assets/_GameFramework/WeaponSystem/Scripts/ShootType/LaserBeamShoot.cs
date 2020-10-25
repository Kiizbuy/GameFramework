using GameFramework.Components;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public class LaserBeamShoot : IShootType
    {
        [SerializeField] private Transform _beamStartPoint;
        [SerializeField] private LineRenderer _laserLine;
        [SerializeField] private float _lazerBeamRange;
        [SerializeField] private float _minBeamWidth = 2f;
        [SerializeField] private float _maxBeamWidth = 2f;
        [SerializeField] private bool _useReflect;
        [SerializeField] private int _maxReflectionsCount = 2;

        public void ShootAndTryTakeDamage(int damage, IAttackable attackable)
        {
            _laserLine.startWidth = _minBeamWidth;
            _laserLine.endWidth = _maxBeamWidth;

            var reflections = 0;
            var reflectionPoints = new List<Vector3>();

            reflectionPoints.Add(_beamStartPoint.position);

            var lastPoint = _beamStartPoint.position;
            var keepReflecting = true;

            Vector3 incomingDirection;
            Vector3 reflectDirection;
            var ray = new Ray(lastPoint, _beamStartPoint.forward);

            do
            {
                // Initialize the next point.  If a raycast hit is not returned, this will be the forward direction * range
                var nextPoint = ray.direction * _lazerBeamRange;

                if (Physics.Raycast(ray, out var hit, _lazerBeamRange))
                {
                    nextPoint = hit.point;
                    incomingDirection = nextPoint - lastPoint;
                    reflectDirection = Vector3.Reflect(incomingDirection, hit.normal);
                    ray = new Ray(nextPoint, reflectDirection);

                    lastPoint = hit.point;

                    //// Hit Effects
                    //if (makeHitEffects)
                    //{
                    //    foreach (GameObject hitEffect in hitEffects)
                    //    {
                    //        if (hitEffect != null)
                    //            Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                    //    }
                    //}

                    // Damage
                    hit.collider.gameObject.SendMessageUpwards("ChangeHealth", damage, SendMessageOptions.DontRequireReceiver);
                    reflections++;
                }
                else
                {

                    keepReflecting = false;
                }

                // Add the next point to the list of beam reflection points
                reflectionPoints.Add(nextPoint);

            } while (CanReflect(keepReflecting, reflections));

            _laserLine.positionCount = reflectionPoints.Count;

            for (var i = 0; i < reflectionPoints.Count; i++)
                _laserLine.SetPosition(i, reflectionPoints[i]);

            if (reflections < 1)
                StopShoot();
        }

        public void StopShoot()
        {
            _laserLine.positionCount = 0;
        }

        private bool CanReflect(bool keepReflecting, int reflections)
            => _useReflect && keepReflecting && reflections < _maxReflectionsCount;

        public void DrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_beamStartPoint.position, _beamStartPoint.position + _beamStartPoint.forward * _lazerBeamRange);
        }
    }
}
