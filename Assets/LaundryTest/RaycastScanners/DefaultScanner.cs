using UnityEngine;

namespace LaundryTest.RaycastScanners
{
    public class DefaultScanner : IRaycastScanner
    {
        public RaycastHit LastHitInfo { get; private set; }
        
        public Transform CameraTransform {get;set;}
        public float Distance {get;set;}
        public string LayerName {get;set;}
        public string ObjectTag {get;set;}

        public DefaultScanner(Transform cameraTransform, float distance, string layerName, string objectTag)
        {
            CameraTransform = cameraTransform;
            Distance = distance;
            LayerName = layerName;
            ObjectTag = objectTag;
        }

        public T ScanForObject<T>() where T : MonoBehaviour
        {
            if (Physics.Raycast(
                    CameraTransform.position,
                    CameraTransform.forward,
                    out var hitInfo,
                    Distance,
                    LayerMask.GetMask(LayerName)))
            {
                LastHitInfo = hitInfo;
                if (hitInfo.transform.CompareTag(ObjectTag) && 
                    hitInfo.transform.TryGetComponent<T>(out var block))
                {
                    return block;
                }
            }

            return null;
        }
    }
}