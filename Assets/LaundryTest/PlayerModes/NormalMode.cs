using LaundryTest.Buildings.Blocks;
using UnityEngine;

namespace LaundryTest.PlayerModes
{
    public class NormalMode : AbstractMode
    {
        public override void Enter(PlayerModeContext context){}

        public override EPlayerMode Update(PlayerModeContext context)
        {
            EPlayerMode result = EPlayerMode.Normal;
            
            context.LookAtObject = GetLookAtObject(context.PlayerCameraTransform, context.PlayerProperties.GrabDistanceRange.max);
            
            if (context.LookAtObject != null)
            {
                if (context.PlayerInput.InteractPerformed) 
                {
                    result = EPlayerMode.Build;
                }
            }

            return result;
        }

        public override void Exit(PlayerModeContext context){}

        private BuildBlock GetLookAtObject(Transform cameraTransform, float distance)
        {
            if (Physics.Raycast(
                    cameraTransform.position,
                    cameraTransform.forward,
                    out var hitInfo,
                    distance,
                    LayerMask.GetMask(GameplayLayers.GrabableLayerName)))
            {
                return hitInfo.transform.TryGetComponent<BuildBlock>(out var block) ? block : null;
            }

            return null;
        }
    }
}