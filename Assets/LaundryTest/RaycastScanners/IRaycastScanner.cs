using UnityEngine;

namespace LaundryTest.RaycastScanners
{
    public interface IRaycastScanner
    {
        public RaycastHit LastHitInfo { get; }
        public T ScanForObject<T>() where T : MonoBehaviour;
    }
}