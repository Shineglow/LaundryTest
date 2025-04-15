using UnityEngine;

namespace LaundryTest.NewWay
{
    public static class Vector3Utils
    {
        public static readonly Vector3 nonUp = Vector3.one - Vector3.up;
        
        public static Vector3 MultiplyComponents(this Vector3 vec1, Vector3 vec2)
        {
            return new Vector3(vec1.x * vec2.x, vec1.y * vec2.y, vec1.z * vec2.z);
        }
    }
}