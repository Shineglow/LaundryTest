using System;
using UnityEngine;

namespace LaundryTest.NewWay
{
    public class TestCollisions : MonoBehaviour
    {
        private ContactPoint[] _contacts;

        private void OnCollisionStay(Collision other)
        {
            _contacts = other.contacts;
        }

        private void OnDrawGizmos()
        {
            if (_contacts != null)
            {
                Gizmos.color = Color.red;
                foreach (var contact in _contacts)
                {
                    Gizmos.DrawLine(contact.point, contact.point + contact.normal*0.25f);
                    break;
                }
            }
        }
    }
}