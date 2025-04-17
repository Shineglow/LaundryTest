using UnityEngine;

namespace LaundryTest.Buildings.Blocks
{
    public sealed class CubeBlock : BuildBlock
    {
        private BoxCollider scannerTrigger;
        
        protected override void Awake()
        {
            base.Awake();

            scannerTrigger = new GameObject(nameof(scannerTrigger)).AddComponent<BoxCollider>();
            scannerTrigger.isTrigger = true;
            scannerTrigger.gameObject.layer = LayerMask.NameToLayer(GameplayLayers.SurfaceDetector);
            SetScannerEnabled(false);
            TryPlaceToGround();
            scannerTrigger.transform.parent = transform;
        }

        public override bool CastForm(Vector3 position, Vector3 direction, float maxDistance, out RaycastHit hitInfo, int layerMask)
        {
            return Physics.BoxCast(position, collider.bounds.extents, direction, out hitInfo, transform.rotation, maxDistance, layerMask);
        }

        protected override void SetScannerEnabled(bool isEnabled)
        {
            scannerTrigger.enabled = isEnabled;
        }
    }
}