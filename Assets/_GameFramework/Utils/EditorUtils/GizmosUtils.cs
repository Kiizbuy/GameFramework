using UnityEngine;

namespace GameFramework.Utils.Gizmos
{
    public static class GizmosUtils
    {
        public static void DrawWireDisk(Vector3 position, Quaternion rotation, float radius, Color color)
        {
            var oldColor = global::UnityEngine.Gizmos.color;
            var oldMatrix = global::UnityEngine.Gizmos.matrix;

            global::UnityEngine.Gizmos.color = color;
            global::UnityEngine.Gizmos.matrix = Matrix4x4.TRS(position, rotation, new Vector3(1, 0.01f, 1));
            global::UnityEngine.Gizmos.DrawWireSphere(Vector3.zero, radius);
            global::UnityEngine.Gizmos.matrix = oldMatrix;
            global::UnityEngine.Gizmos.color = oldColor;
        }
    }
}
