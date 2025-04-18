using System;
using LaundryTest.Buildings.Blocks;
using LaundryTest.Spawners;
using Unity.VisualScripting;
using UnityEngine;

namespace LaundryTest.PlayerModes
{
    public class NormalMode : AbstractMode
    {
        public override void Enter(PlayerModeContext context)
        {
            if (!context.UsedSpawnerProperties.Spawner.IsUnityNull())
            {
                if (context.UsedSpawnerProperties.WasCancelled)
                {
                    context.UsedSpawnerProperties.Spawner.Cancel();
                    context.UsedSpawnerProperties.WasCancelled = false;
                }
                else if (context.UsedSpawnerProperties.WasApplied)
                {
                    context.UsedSpawnerProperties.Spawner.Apply();
                    context.UsedSpawnerProperties.WasCancelled = false;
                }

                context.UsedSpawnerProperties.Spawner = null;
            }
        }

        public override EPlayerMode Update(PlayerModeContext context)
        {
            EPlayerMode result = EPlayerMode.Normal;

            var isLookAtSomething = GetLookAtObject(context.PlayerCameraTransform,
                context.PlayerProperties.GrabDistanceRange.max, out context.LookAtObject, out var spawner);

            if (isLookAtSomething)
            {
                if (context.PlayerInput.InteractPerformed.Value && context.PlayerInput.InteractPerformed.WasChanged)
                {
                    if (context.LookAtObject != null)
                    {
                        if (context.LookAtObject.PuttedOnObject == null)
                        {
                            if (context.LookAtObject.UnderObject != null)
                            {
                                context.CachedPreviousUnderObject = context.LookAtObject.UnderObject;
                                BuildBlock.ClearVerticalRelations(context.CachedPreviousUnderObject, context.LookAtObject);
                            }

                            result = GetBuildModeByBlockType(context.LookAtObject.BlockType);
                        }
                    }
                    else if (spawner != null)
                    {
                        BuildBlock buildObject = spawner.Spawn();
                        context.LookAtObject = buildObject;
                        context.InitialPosition.Value = buildObject.transform.position;
                        context.InitialRotation.Value = buildObject.transform.rotation;

                        context.UsedSpawnerProperties.Spawner = spawner;
                        
                        result = GetBuildModeByBlockType(context.LookAtObject.BlockType);;
                    }
                }
            }

            return result;
        }

        private static EPlayerMode GetBuildModeByBlockType(EBuildBlockType blockType)
        {
            return blockType switch
            {
                EBuildBlockType.Cube => EPlayerMode.BuildCube,
                EBuildBlockType.Circle => EPlayerMode.BuildCircle,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override void Exit(PlayerModeContext context){}

        private bool GetLookAtObject(Transform cameraTransform, float distance, out BuildBlock block,
            out AbstractSpawner spawner)
        {
            bool result = false;
            block = null;
            spawner = null;

            if (Physics.Raycast(
                    cameraTransform.position,
                    cameraTransform.forward,
                    out var hitInfo,
                    distance,
                    LayerMask.GetMask(GameplayLayers.GrabableLayerName, GameplayLayers.Spawner)))
            {
                if (hitInfo.transform.TryGetComponent(out block))
                {
                    result = true;
                }
                else if (hitInfo.transform.TryGetComponent(out spawner))
                {
                    result = true;
                }
            }

            return result;
        }
    }
}