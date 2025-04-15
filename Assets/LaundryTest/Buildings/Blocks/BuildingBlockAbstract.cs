using Unity.VisualScripting;
using UnityEngine;

namespace LaundryTest
{
    public static class BuildingBlockAbstract
    {
        public static Vector3 GetOffsetToIntersection(BoxCollider collider1, MeshCollider collider2)
        {
            return Vector3.zero;
        }

        public static Vector3 GetOffsetToIntersection(BoxCollider collider1, BoxCollider collider2, out RaycastHit hitInfo)
        {
            if (Physics.BoxCast(
                    collider1.transform.position, 
                    collider1.size / 2,
                    collider1.transform.TransformDirection(Vector3.down), 
                    out hitInfo,
                    collider1.transform.rotation))
            {
                if (hitInfo.collider != collider2) return Vector3.zero;
            }

            return Vector3.zero;
        }

        public static Vector3 GetOffsetToIntersection(MeshCollider collider1, MeshCollider collider2)
        {
            return Vector3.zero;
        }
    }
}
