using LaundryTest.Buildings.Blocks;
using UnityEngine;

namespace LaundryTest.Spawners
{
    [RequireComponent(typeof(MeshCollider))]
    public class CircleSpawner : AbstractSpawner
    {
        [SerializeField] private СircularBlock prefab;

        [SerializeField] private Transform targetTransform;
        
        protected override BuildBlock CreateInstance()
        {
            return Instantiate(prefab, targetTransform.position, targetTransform.rotation);
        }

        protected override void SetInitialTransformations(BuildBlock block)
        {
            block.transform.rotation = targetTransform.rotation;
            block.transform.position = targetTransform.position;
        }
    }
}