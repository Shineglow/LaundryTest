using LaundryTest.Buildings.Blocks;
using Unity.VisualScripting;
using UnityEngine;

namespace LaundryTest.PlayerModes
{
    public class BuildCircle : AbstractMode
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
            EPlayerMode result = EPlayerMode.BuildCircle;
            RaycastHit hitInfo;
            
            var hitResult = context.ObjectInHands.CastForm(context.PlayerCameraTransform.position, context.PlayerCameraTransform.forward,
                context.PlayerProperties.GrabDistanceRange.max, out hitInfo,
                LayerMask.GetMask(GameplayLayers.Wall, GameplayLayers.Default, GameplayLayers.GrabableLayerName));

            float distance;
            if (hitResult)
            {
                distance = hitInfo.distance;
                context.ObjectInHands.AlignForwardWith(hitInfo.normal);
            }
            else
            {
                distance = _distanceToObject.Value;
            }

            context.ObjectInHands.MoveObject(context.PlayerCameraTransform.position +
                                             context.PlayerCameraTransform.forward * distance);


            if (context.PlayerInput.InteractPerformed is { Value: true, WasChanged: true })
            {
                if (context.ObjectInHands.CanBePlacedNow())
                {
                    context.ObjectInHands.SetGrabbed(false);
                    if (!context.UsedSpawnerProperties.Spawner.IsUnityNull())
                    {
                        context.UsedSpawnerProperties.WasApplied = true;
                    }
                    
                    result = EPlayerMode.Normal;
                }
            }
            else if (context.PlayerInput.CancelPerformed is { Value: true, WasChanged: true })
            {
                if (!context.UsedSpawnerProperties.Spawner.IsUnityNull())
                {
                    context.UsedSpawnerProperties.WasCancelled = true;
                }
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