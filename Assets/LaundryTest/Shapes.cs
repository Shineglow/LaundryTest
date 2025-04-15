
using UnityEngine;

namespace LaundryTest
{
    public static class Shapes
    {
        public static Vector3[] GetParallelepiped(Vector3 size)
        {
            Vector3 halfSize = size * 0.5f;
            return new Vector3[]
            {
                new(-halfSize.x, -halfSize.y, -halfSize.z),
                new(halfSize.x, -halfSize.y, -halfSize.z),
                new(halfSize.x, -halfSize.y, halfSize.z),
                new(-halfSize.x, -halfSize.y, halfSize.z),

                new(-halfSize.x, halfSize.y, -halfSize.z),
                new(halfSize.x, halfSize.y, -halfSize.z),
                new(halfSize.x, halfSize.y, halfSize.z),
                new(-halfSize.x, halfSize.y, halfSize.z),
            };
        }
        
        public static Vector3[] GetQuad(Vector2 size)
        {
            Vector2 halfSize = size * 0.5f;
            return new Vector3[]
            {
                new(-halfSize.x, 0f, -halfSize.y),
                new(halfSize.x, 0f, -halfSize.y),
                new(halfSize.x, 0f, halfSize.y),
                new(-halfSize.x, 0f, halfSize.y),
            };
        }

        public static Vector3[] GetCylinder(float height, int segments, float radius)
        {
            float halfHeight = height / 2f;
            Vector3 topCenter = Vector3.up * halfHeight;
            Vector3 bottomCenter = Vector3.up * -halfHeight;

            float angleStep = 360f / segments;
            Vector3[] points = new Vector3[segments*2];

            for (int i = 0; i < segments; i++)
            {
                float angle = Mathf.Deg2Rad * i * angleStep;
                Vector3 localPoint = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                points[i] = topCenter + localPoint;
                points[segments + i] = bottomCenter + localPoint;
            }

            return points;
        }
        
        public static Vector3[] GetCircle(int segments, float radius)
        {
            float angleStep = 360f / segments;
            Vector3[] points = new Vector3[segments];

            for (int i = 0; i < segments; i++)
            {
                float angle = Mathf.Deg2Rad * i * angleStep;
                points[i] = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            }

            return points;
        }
        
        public static void GetParallelepiped(Vector3 size, ref Vector3[] result)
        {
            Vector3 halfSize = size / 0.5f;
            result[0] = new(-halfSize.x, -halfSize.y, -halfSize.z);
            result[1] = new(halfSize.x, -halfSize.y, -halfSize.z);
            result[2] = new(halfSize.x, -halfSize.y, halfSize.z);
            result[3] = new(-halfSize.x, -halfSize.y, halfSize.z);
            result[4] = new(-halfSize.x, halfSize.y, -halfSize.z);
            result[5] = new(halfSize.x, halfSize.y, -halfSize.z);
            result[6] =  new(halfSize.x, halfSize.y, halfSize.z);
            result[7] = new(-halfSize.x, halfSize.y, halfSize.z);
        }
        
        public static void GetQuad(Vector3 size, ref Vector3[] result)
        {
            Vector3 halfSize = size / 0.5f;
            result[0] = new(-halfSize.x, 0f, -halfSize.z);
            result[1] = new(halfSize.x, 0f, -halfSize.z);
            result[2] = new(halfSize.x, 0f, halfSize.z);
            result[3] = new(-halfSize.x, 0f, halfSize.z);
        }

        public static void GetCylinder(float height, float radius, ref Vector3[] result)
        {
            float halfHeight = height / 2f;
            Vector3 topCenter = Vector3.up * halfHeight;
            Vector3 bottomCenter = Vector3.up * -halfHeight;

            int segments = result.Length / 2;
            float angleStep = 360f / segments;

            for (int i = 0; i < segments; i++)
            {
                float angle = Mathf.Deg2Rad * i * angleStep;
                Vector3 localPoint = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                result[i] = topCenter + localPoint;
                result[segments + i] = bottomCenter + localPoint;
            }
        }
        
        public static void GetCircle(float radius, ref Vector3[] result)
        {
            int segments = result.Length;
            float angleStep = 360f / segments;

            for (int i = 0; i < segments; i++)
            {
                float angle = Mathf.Deg2Rad * i * angleStep;
                result[i] = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            }
        }
    }
}