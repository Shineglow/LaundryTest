using UnityEngine;

namespace LaundryTest.Buildings.Blocks
{
    public sealed class CubeBlock : BuildBlock
    {
        [SerializeField] private new BoxCollider collider;
        [SerializeField] private BoxCollider groundCheckCollider;
        private int _actualFrame = -1;
        private bool _canBePlacedCached;

        public override EBuildBlockType BlockType => EBuildBlockType.Cube;
        private int _placedOnMask;

        public override bool CanPutOn() => true;
        public override bool CanBePlacedOnGround() => true;
        public override bool CanBePlacedOnWall() => false;
        public override bool CanBePlacedOn(int layerMask)
        {
            return (_placedOnMask & layerMask) != 0;
        }

        protected override void Awake()
        {
            base.Awake();
            _placedOnMask = LayerMask.GetMask(GameplayLayers.Default, GameplayLayers.GrabableLayerName);
            TryPinDown(out var buildBlock);
            SetAUnderB(this, buildBlock);
        }

        public override bool CastForm(Vector3 position, Vector3 direction, float maxDistance, out RaycastHit hitInfo, int layerMask)
        {
            return Physics.BoxCast(position, collider.size/2f, direction, out hitInfo, transform.rotation, maxDistance, layerMask);
        }

        public override void AlignForwardWith(Vector3 direction)
        {
            RotateObject(Quaternion.LookRotation(direction, Vector3.up));
        }

        public override bool TrySnapOnTop(BuildBlock buildBlock)
        {
            if(!CanBePlacedNow()) return false;
            if (buildBlock.BlockType == BlockType)
            {
                var targetPos = buildBlock.transform.position;
                targetPos.y = rb.position.y;
                rb.Move(targetPos, buildBlock.transform.rotation);

                return true;
            }

            return false;
        }

        public override bool CanBePlacedNow()
        {
            if (_actualFrame == Time.frameCount) return _canBePlacedCached;
            
            _actualFrame = Time.frameCount;
            Vector3 worldCenter = groundCheckCollider.transform.TransformPoint(groundCheckCollider.center);
            Quaternion rotation = groundCheckCollider.transform.rotation;
            Collider[] overlaps = Physics.OverlapBox(worldCenter, groundCheckCollider.size / 2, rotation, 
                LayerMask.GetMask(GameplayLayers.Default, GameplayLayers.GrabableLayerName));

            if (overlaps.Length == 0)
            {
                _canBePlacedCached = false;
            }
            else
            {
                var isGrabableUnderObject = overlaps[0].gameObject.layer == LayerMask.NameToLayer(GameplayLayers.GrabableLayerName);
                var isDefaultUnderObject = overlaps[0].gameObject.layer == LayerMask.NameToLayer(GameplayLayers.Default);
                _canBePlacedCached = (isGrabableUnderObject || isDefaultUnderObject);

                if (isGrabableUnderObject && overlaps[0].gameObject.TryGetComponent<BuildBlock>(out var placedOnObj))
                {
                    _canBePlacedCached &= placedOnObj.CanPutOn();
                }
            }

            return _canBePlacedCached;
        }
    }
}