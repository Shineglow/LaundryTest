using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Assets.LaundryTest.Buildings.Blocks
{
    public class BuildBlock : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private new Collider collider;

        private HashSet<Collision> _colidedNow = new HashSet<Collision>();

        public void MoveObject(Vector3 targetPosition, bool isForce = false)
        {
            Vector3 offset = Vector3.zero;

            OffsetCorrection(isForce, ref offset);

            rb.MovePosition(targetPosition + offset);
        }

        private void OffsetCorrection(bool isForce, ref Vector3 offset)
        {
            if (!isForce)
            {
                offset = GetPenetrationOffset(collider, transform.position, transform.rotation, LayerMask.GetMask("Default"));

                /*foreach (var item in _colidedNow)
                {
                    offset -= GetOffset(item.collider, item.transform.position, item.transform.rotation);
                }*/
            }
        }

        public void RotateObject(Quaternion targetRotation, bool isForce = false)
        {
            Vector3 offset = Vector3.zero;

            rb.MoveRotation(targetRotation);

            OffsetCorrection(isForce, ref offset);

            rb.Move(rb.position + offset, targetRotation);
        }

        public Vector3 GetOffset(Collider colliderB, Vector3 positionB, Quaternion rotationB) 
        {
            if(Physics.ComputePenetration(collider, transform.position, transform.rotation, colliderB, positionB, rotationB, out Vector3 direction, out float distance))
            {
                return direction*distance;
            }
            return Vector3.zero;
        }

        public void SetCollisionsEnabled(bool enabled)
        {
            rb.detectCollisions = enabled;
            collider.enabled = enabled;
        }

        private Vector3 GetPenetrationOffset(
            Collider collider,
            Vector3 position,
            Quaternion rotation,
            LayerMask layerMask)
        {
            Vector3 totalOffset = Vector3.zero;

            Collider[] overlaps = Physics.OverlapBox(
                position,
                collider.bounds.extents,
                rotation,
                layerMask,
                QueryTriggerInteraction.Ignore);

            foreach (var other in overlaps)
            {
                if (other == collider)
                    continue;

                bool hasPenetration = Physics.ComputePenetration(
                    collider, position, rotation,
                    other, other.transform.position, other.transform.rotation,
                    out var direction, out var distance);

                if (hasPenetration)
                {
                    totalOffset += direction.normalized * distance;
                }
            }

            return totalOffset;
        }

        /*int updatesCounter = 0;
        private void FixedUpdate()
        {
            updatesCounter++;
            if(updatesCounter == 10)
            {
                Vector3 offset = Vector3.zero;

                OffsetCorrection(false, ref offset);

                Debug.Log(offset);

                updatesCounter = 0;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_colidedNow.Contains(collision))
            {
                _colidedNow.Add(collision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (_colidedNow.Contains(collision))
            {
                _colidedNow.Remove(collision);
            }
        }*/
    }
}
