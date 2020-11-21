using UnityEngine;

namespace GameFramework.UI
{
    public interface IMarker
    {
        void SetPosition(ICameraView camera, Vector3 position);
    }
}
