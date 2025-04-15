using System.Collections.Generic;
using UnityEngine;

namespace LaundryTest
{
    public static class GizmosCustom
    {
        public static void DrawGizmoCylinder(Vector3 position, float radius, float height, int segments, Color color)
        {
            DrawGizmoCylinder(position, radius, height,  segments, color, Matrix4x4.identity);
        }
        
        public static void DrawGizmoCylinder(Vector3 position, float radius, float height, int segments, Color color, Matrix4x4 rotationMatrix)
        {
            Gizmos.color = color;

            float halfHeight = height / 2f;
            Vector3 up = rotationMatrix.MultiplyVector(Vector3.up);
            Vector3 topCenter = position + up * halfHeight;
            Vector3 bottomCenter = position - up * halfHeight;

            float angleStep = 360f / segments;
            Vector3[] topPoints = new Vector3[segments];
            Vector3[] bottomPoints = new Vector3[segments];

            for (int i = 0; i < segments; i++)
            {
                float angle = Mathf.Deg2Rad * i * angleStep;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                Vector3 localPoint = new Vector3(x, 0, z);
                Vector3 rotatedPoint = rotationMatrix.MultiplyVector(localPoint);

                topPoints[i] = topCenter + rotatedPoint;
                bottomPoints[i] = bottomCenter + rotatedPoint;
            }

            for (int i = 0; i < segments; i++)
            {
                int next = (i + 1) % segments;

                Gizmos.DrawLine(topPoints[i], topPoints[next]);

                Gizmos.DrawLine(bottomPoints[i], bottomPoints[next]);

                Gizmos.DrawLine(topPoints[i], bottomPoints[i]);
            }
        }
        
        public static void DrawGizmoCylinder(Vector3[] points, Vector3 position, Color color)
        {
            Gizmos.color = color;
            int segments = points.Length / 2;

            for (int i = 0; i < segments; i++)
            {
                int next = (i + 1) % segments;

                Gizmos.DrawLine(position + points[i], position + points[next]);

                Gizmos.DrawLine(position + points[segments + i], position + points[segments + next]);

                Gizmos.DrawLine(position + points[i], position + points[segments + i]);
            }
        }

        public static void DrawGizmoCube(Vector3 position, Vector3 size, Color color)
        {
            DrawGizmoCube(position, size, color, Matrix4x4.identity);
        }
        
        public static void DrawGizmoCube(Vector3 position, Vector3 size, Color color, Matrix4x4 rotationMatrix)
        {
            Gizmos.color = color;

            Vector3 halfSize = size * 0.5f;

            // Локальные вершины куба
            Vector3[] localCorners = new Vector3[8]
            {
                new (-halfSize.x, -halfSize.y, -halfSize.z),
                new ( halfSize.x, -halfSize.y, -halfSize.z),
                new ( halfSize.x, -halfSize.y,  halfSize.z),
                new (-halfSize.x, -halfSize.y,  halfSize.z),

                new (-halfSize.x,  halfSize.y, -halfSize.z),
                new ( halfSize.x,  halfSize.y, -halfSize.z),
                new ( halfSize.x,  halfSize.y,  halfSize.z),
                new (-halfSize.x,  halfSize.y,  halfSize.z),
            };

            for (int i = 0; i < localCorners.Length; i++)
            {
                localCorners[i] = position + rotationMatrix.MultiplyVector(localCorners[i]);
            }
            
            Gizmos.DrawLine(localCorners[0], localCorners[1]);
            Gizmos.DrawLine(localCorners[1], localCorners[2]);
            Gizmos.DrawLine(localCorners[2], localCorners[3]);
            Gizmos.DrawLine(localCorners[3], localCorners[0]);

            Gizmos.DrawLine(localCorners[4], localCorners[5]);
            Gizmos.DrawLine(localCorners[5], localCorners[6]);
            Gizmos.DrawLine(localCorners[6], localCorners[7]);
            Gizmos.DrawLine(localCorners[7], localCorners[4]);
            
            Gizmos.DrawLine(localCorners[0], localCorners[4]);
            Gizmos.DrawLine(localCorners[1], localCorners[5]);
            Gizmos.DrawLine(localCorners[2], localCorners[6]);
            Gizmos.DrawLine(localCorners[3], localCorners[7]);
        }
        
        public static void DrawGizmoCube(Vector3[] points, Vector3 position, Color color)
        {
            Gizmos.color = color;
            
            Gizmos.DrawLine(position + points[0], position + points[1]);
            Gizmos.DrawLine(position + points[1], position + points[2]);
            Gizmos.DrawLine(position + points[2], position + points[3]);
            Gizmos.DrawLine(position + points[3], position + points[0]);

            Gizmos.DrawLine(position + points[4], position + points[5]);
            Gizmos.DrawLine(position + points[5], position + points[6]);
            Gizmos.DrawLine(position + points[6], position + points[7]);
            Gizmos.DrawLine(position + points[7], position + points[4]);
            
            Gizmos.DrawLine(position + points[0], position + points[4]);
            Gizmos.DrawLine(position + points[1], position + points[5]);
            Gizmos.DrawLine(position + points[2], position + points[6]);
            Gizmos.DrawLine(position + points[3], position + points[7]);
        }
        
        public static void DrawGizmoQuad(Vector3[] points, Vector3 position, Color color)
        {
            Gizmos.color = color;
            
            Gizmos.DrawLine(position + points[0], position + points[1]);
            Gizmos.DrawLine(position + points[1], position + points[2]);
            Gizmos.DrawLine(position + points[2], position + points[3]);
            Gizmos.DrawLine(position + points[3], position + points[0]);
        }
        public static void DrawGizmoCircle(List<Vector3> points, Vector3 position, Color color)
        {
            Gizmos.color = color;
            int segments = points.Count;

            for (int i = 0; i < segments; i++)
            {
                int next = (i + 1) % segments;

                Gizmos.DrawLine(position + points[i], position + points[next]);
            }
        }
    }

    public enum EGizmosForm
    {
        Sphere,
        Cube,
        Сylinder,
        Quad,
        Circle,
    }
}