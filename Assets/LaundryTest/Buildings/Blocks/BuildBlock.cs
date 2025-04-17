using UnityEngine;

namespace LaundryTest.Buildings.Blocks
{
    public abstract class BuildBlock : MonoBehaviour, IBuildBlock
    {
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected new Collider collider;
        private bool _isGrabbed;

        protected virtual void Awake(){}

        public void MoveObject(Vector3 targetPosition, bool isForce = false)
        {
            if(_isGrabbed)
                rb.MovePosition(targetPosition);
        }

        public void RotateObject(Quaternion targetRotation, bool isForce = false)
        {
            if(_isGrabbed)
                rb.MoveRotation(targetRotation);
        }

        public abstract bool CastForm(Vector3 position, Vector3 direction, float maxDistance, out RaycastHit hitInfo, int layerMask);

        public void SetGrabbed(bool isGrabbed)
        {
            if (isGrabbed == _isGrabbed) return;
            
            _isGrabbed = isGrabbed;
            gameObject.layer = isGrabbed
                ? LayerMask.NameToLayer(GameplayLayers.GrabbedLayerName)
                : LayerMask.NameToLayer(GameplayLayers.GrabableLayerName);
            SetScannerEnabled(isGrabbed);
        }

        public bool TryPlaceToGround()
        {
            bool result = false;
            var isGrabbed = _isGrabbed;
            SetGrabbed(true);
            
            if(CastForm(
                   transform.position, 
                   Vector3.down, 
                   float.PositiveInfinity, 
                   out var hitInfo, 
                   LayerMask.GetMask(GameplayLayers.Default, GameplayLayers.GrabableLayerName)))
            {
                result = true;
                MoveObject(transform.position += Vector3.down * hitInfo.distance);
            }
            
            SetGrabbed(isGrabbed);
            return result;
        }

        protected abstract void SetScannerEnabled(bool isEnabled);
    }
}
