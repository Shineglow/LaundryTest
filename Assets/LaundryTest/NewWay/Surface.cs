using System.Collections.Generic;
using UnityEngine;

namespace LaundryTest.NewWay
{
    public class Surface : MonoBehaviour
    {
        public Vector3 Right => transform.right;
        public Vector3 Normal => transform.up;

        private List<Vector3> points;
        private List<Vector3> pointsTransformed;
        
        private bool _changePointsTransformation;
        public Vector3 Position
        {
            get => transform.position;
            set
            {
                transform.position = value;
                _changePointsTransformation = true;
            }
        }

        public Quaternion Rotation
        {
            get => transform.rotation;
            set
            {
                transform.rotation = value;
                _changePointsTransformation = true;
            }
        }

        [field: SerializeField] public float Radius { get; set; }
        [field: SerializeField] public float Width { get; set; }
        [field: SerializeField] public float Height { get; set; }

        [field: SerializeField] public EGizmosForm GizmosForm { get; set; }
        [field: SerializeField] public Color GizmosColor { get; set; } = Color.red;

        private void OnValidate()
        {
            if (_changePointsTransformation)
            {
                pointsTransformed.AlignCapacityWithCount(points);
                pointsTransformed.Clear();
                foreach (var point in points)
                {
                    pointsTransformed.Add(transform.localToWorldMatrix.MultiplyPoint3x4(point));
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (points is {Count:0} ) return;

            Gizmos.color = GizmosColor;
            GizmosCustom.DrawGizmoCircle(pointsTransformed, Vector3.zero, GizmosColor);
            Gizmos.DrawLine(transform.position, transform.position + transform.up * 0.25f);
        }

        public void SetPoints(List<Vector3> newPoints)
        {
            points.AlignCapacityWithCount(newPoints);
            points.Clear();
            pointsTransformed.AlignCapacityWithCount(newPoints);
            pointsTransformed.Clear();
            foreach (var point in newPoints)
            {
                points.Add(point);
                pointsTransformed.Add(transform.localToWorldMatrix.MultiplyPoint3x4(point));
            }
        }
    }
}