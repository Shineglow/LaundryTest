using UnityEngine;

namespace LaundryTest.PlayerModes
{
    public class BuildMode : AbstractMode
    {
        private InitializableProperty<float> _distanceToObject;

        public override void Enter(PlayerModeContext context)
        {
            if (context.LookAtObject != null)
            {
                context.ObjectInHands = context.LookAtObject;
                context.InitialPosition.Value = context.ObjectInHands.transform.position;
                context.InitialRotation.Value = context.ObjectInHands.transform.rotation;
                context.ObjectInHands.SetGrabbed(true);
                _distanceToObject.Value = Vector3.Distance(context.PlayerCameraTransform.position,context.LookAtObject.transform.position);
            }
        }

        public override EPlayerMode Update(PlayerModeContext context)
        {
            EPlayerMode result = EPlayerMode.Build;
            
            if (context.ObjectInHands != null)
            {
                var hitResult = context.ObjectInHands.CastForm(context.PlayerCameraTransform.position, context.PlayerCameraTransform.forward,
                    context.PlayerProperties.GrabDistanceRange.max, out var hitInfo,
                    ~LayerMask.GetMask(GameplayLayers.GrabbedLayerName, GameplayLayers.Player,
                        GameplayLayers.SurfaceDetector));
                var distance = hitResult ? hitInfo.distance : _distanceToObject.Value;
                context.ObjectInHands.MoveObject(context.PlayerCameraTransform.position + context.PlayerCameraTransform.forward * distance);
            }

            if (context.PlayerInput.InteractPerformed)
            {
                context.ObjectInHands.SetGrabbed(false);
                result = EPlayerMode.Normal;
            }
            else if (context.PlayerInput.CancelPerformed) /* cancel performed */
            {
                context.ObjectInHands.MoveObject(context.InitialPosition.Value);
                context.ObjectInHands.RotateObject(context.InitialRotation.Value);
                context.ObjectInHands.SetGrabbed(false);
                result = EPlayerMode.Normal;
            }

            return result;
        }

        public override void Exit(PlayerModeContext context)
        {
            context.InitialPosition.Reset();
            context.InitialRotation.Reset();
            context.ObjectInHands = null;
        }
    }
}