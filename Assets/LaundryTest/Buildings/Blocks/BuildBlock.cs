using UnityEngine;

namespace LaundryTest.Buildings.Blocks
{
    public abstract class BuildBlock : MonoBehaviour, IBuildBlock
    {
        public abstract EBuildBlockType BlockType { get; }
        
        [SerializeField] protected Rigidbody rb;
        private bool _isGrabbed;

        [SerializeField] private MeshRenderer mr;
        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material buildAllowedMaterial;
        [SerializeField] private Material buildForbiddenMaterial;

        public BuildBlock PuttedOnObject { get; private set; }
        public BuildBlock UnderObject { get; private set; }
        public abstract bool CanPutOn();
        public abstract bool CanBePlacedOnGround();
        public abstract bool CanBePlacedOnWall();
        public abstract bool CanBePlacedOn(int layerMask);

        protected virtual void Awake(){}

        public virtual void SetPuttedOnObject(BuildBlock puttedOnObject)
        {
            PuttedOnObject = puttedOnObject;
        }

        public void MoveObject(Vector3 targetPosition, bool isForce = false)
        {
            if (_isGrabbed)
            {
                rb.MovePosition(targetPosition);
                UpdateColorHint();
            }
        }

        public void RotateObject(Quaternion targetRotation, bool isForce = false)
        {
            if (_isGrabbed)
            {
                rb.MoveRotation(targetRotation);
            }
        }

        public abstract bool CastForm(Vector3 position, Vector3 direction, float maxDistance, out RaycastHit hitInfo, int layerMask);

        public abstract void AlignForwardWith(Vector3 direction);

        public abstract bool TrySnapOnTop(BuildBlock buildBlock);

        public void SetGrabbed(bool isGrabbed)
        {
            if (isGrabbed == _isGrabbed) return;
            
            _isGrabbed = isGrabbed;
            if (_isGrabbed)
            {
                UpdateColorHint();
                gameObject.layer = LayerMask.NameToLayer(GameplayLayers.GrabbedLayerName);
            }
            else
            {
                mr.material = normalMaterial;
                gameObject.layer =  LayerMask.NameToLayer(GameplayLayers.GrabableLayerName);
            }
        }

        public bool TryPinDown(out BuildBlock buildObjectComponent)
        {
            bool result = false;
            buildObjectComponent = null;
            var isGrabbed = _isGrabbed;
            SetGrabbed(true);
            
            Physics.SyncTransforms();
            if(CastForm(
                   rb.position, 
                   Vector3.down, 
                   float.PositiveInfinity, 
                   out var hitInfo, 
                   LayerMask.GetMask(GameplayLayers.Default, GameplayLayers.GrabableLayerName)))
            {
                result = true;
                hitInfo.transform.TryGetComponent(out buildObjectComponent);
                MoveObject(rb.position + Vector3.down * hitInfo.distance);
            }
            
            SetGrabbed(isGrabbed);
            return result;
        }

        public void UpdateColorHint()
        {
            mr.material = CanBePlacedNow() ? buildAllowedMaterial : buildForbiddenMaterial;
        }

        public abstract bool CanBePlacedNow();

        public virtual void SetUnderObject(BuildBlock buildBlockComponent)
        {
            UnderObject = buildBlockComponent;
        }
        
        public static void SetAUnderB(BuildBlock a, BuildBlock b)
        {
            if (a == null || b == null) return;
            a.SetUnderObject(b);
            b.SetPuttedOnObject(a);
        }
        public static void ClearVerticalRelations(BuildBlock a, BuildBlock b)
        {
            if(a != null)
            {
                a.SetUnderObject(null);
                a.SetPuttedOnObject(null);
            }

            if (b != null)
            {
                b.SetUnderObject(null);
                b.SetPuttedOnObject(null);
            }
        }
    }

    public enum EBuildBlockType
    {
        Cube,
        Circle,
    }
}
