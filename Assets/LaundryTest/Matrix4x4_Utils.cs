using UnityEngine;

namespace LaundryTest
{
    public static class Matrix4x4_Utils
    {
        public static Matrix4x4 BuildPlaceSurfaceMatrix(
            Vector3 localPosition,
            Vector3 localRotationEuler,
            Transform ownerTransform
        )
        {
            Vector3 worldPosition = ownerTransform.TransformPoint(localPosition);
            Quaternion worldRotation = ownerTransform.rotation * Quaternion.Euler(localRotationEuler);
            return Matrix4x4.TRS(worldPosition, worldRotation, Vector3.one);
        }
    }
}