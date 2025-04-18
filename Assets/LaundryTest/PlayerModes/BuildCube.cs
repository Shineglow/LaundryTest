using LaundryTest.Buildings.Blocks;
using Unity.VisualScripting;
using UnityEngine;

namespace LaundryTest.PlayerModes
{
    public class BuildCube : AbstractMode
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
            EPlayerMode result = EPlayerMode.BuildCube;

            var hitResult = context.ObjectInHands.CastForm(context.PlayerCameraTransform.position, context.PlayerCameraTransform.forward,
                context.PlayerProperties.GrabDistanceRange.max, out var hitInfo,
                LayerMask.GetMask(GameplayLayers.GrabableLayerName, GameplayLayers.Default, GameplayLayers.Wall));
            BuildBlock buildBlockComponent = null;
            
            var containsBuildBlock = hitResult && hitInfo.transform.TryGetComponent(out buildBlockComponent);
            
            if (context.PlayerInput.Action2Performed is { Value: true } && containsBuildBlock)
            {
                context.ObjectInHands.TrySnapOnTop(buildBlockComponent);
            }
            else
            {
                var distance = hitResult ? hitInfo.distance : _distanceToObject.Value;
                context.ObjectInHands.MoveObject(context.PlayerCameraTransform.position + context.PlayerCameraTransform.forward * distance);

                if (context.PlayerInput.Action3Performed is { Value: true })
                {
                    context.ObjectInHands.TryPinDown(out buildBlockComponent);
                }
            }

            if (context.PlayerInput.InteractPerformed is { Value: true, WasChanged: true })
            {
                if (context.ObjectInHands.CanBePlacedNow())
                {
                    context.ObjectInHands.SetGrabbed(false);
                    if (containsBuildBlock)
                    {
                        BuildBlock.SetAUnderB(context.ObjectInHands, buildBlockComponent);
                    }

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
                
                if (context.CachedPreviousUnderObject != null)
                {
                    BuildBlock.SetAUnderB(context.ObjectInHands, context.CachedPreviousUnderObject);
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