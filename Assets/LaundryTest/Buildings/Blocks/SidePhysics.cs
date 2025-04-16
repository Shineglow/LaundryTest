using System;
using UnityEngine;

namespace Assets.LaundryTest.Buildings.Blocks
{
    [Serializable]
    public class SidePhysics
    {
        [field: SerializeField] public BoxCollider Collider { get; private set; }
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
    }
}
