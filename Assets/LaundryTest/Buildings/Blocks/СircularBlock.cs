using UnityEngine;

namespace LaundryTest.Buildings.Blocks
{
    public class СircularBlock : BuildBlock
    {
        [SerializeField] private new BoxCollider collider;
        [SerializeField] private BoxCollider groundCheckCollider;
        private int _actualFrame = -1;
        private bool _canBePlacedCached;

        private int _placedOnMask;
        public override EBuildBlockType BlockType => EBuildBlockType.Circle;

        public override bool CanPutOn() => false;

        public override bool CanBePlacedOnGround() => false;

        public override bool CanBePlacedOnWall() => true;

        protected override void Awake()
        {
            base.Awake();
            _placedOnMask = LayerMask.GetMask(GameplayLayers.Default, GameplayLayers.GrabableLayerName);
        }

        public override bool CanBePlacedOn(int layerMask)
        {
            return (_placedOnMask & layerMask) != 0;
        }

        public override bool CastForm(Vector3 position, Vector3 direction, float maxDistance, out RaycastHit hitInfo,
            int layerMask)
        {
            return Physics.BoxCast(position, Vector3.Scale(collider.size, transform.localScale)/2, direction, out hitInfo, transform.rotation,
                maxDistance, layerMask);
        }

        public override void AlignForwardWith(Vector3 direction)
        {
            transform.up = direction;
        }

        public override bool TrySnapOnTop(BuildBlock buildBlock) => false;

        public override bool CanBePlacedNow()
        {
            if (_actualFrame == Time.frameCount) return _canBePlacedCached;

            _actualFrame = Time.frameCount;
            Vector3 worldCenter = groundCheckCollider.transform.TransformPoint(groundCheckCollider.center);
            Quaternion rotation = groundCheckCollider.transform.rotation;
            Collider[] overlaps = Physics.OverlapBox(worldCenter, groundCheckCollider.size / 2, rotation,
                LayerMask.GetMask(GameplayLayers.Wall));

            _canBePlacedCached = overlaps.Length > 0;

            return _canBePlacedCached;
        }
    }
}