using LaundryTest.Buildings.Blocks;
using UnityEngine;

namespace LaundryTest.Spawners
{
    [RequireComponent(typeof(BoxCollider))]
    public class CubeSpawner : AbstractSpawner
    {
        [SerializeField] private CubeBlock prefab;
        
        protected override BuildBlock CreateInstance()
        {
            return Instantiate(prefab, transform.position, transform.rotation);
        }

        protected override void SetInitialTransformations(BuildBlock block)
        {
            block.transform.rotation = transform.rotation;
            block.transform.position = transform.position;
        }
    }
}
