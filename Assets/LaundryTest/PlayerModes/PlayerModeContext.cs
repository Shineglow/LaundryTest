using LaundryTest.Buildings.Blocks;
using LaundryTest.InputSystem;
using LaundryTest.Spawners;
using UnityEngine;

namespace LaundryTest.PlayerModes
{
    public class PlayerModeContext
    {
        public PlayerInput PlayerInput;
        public PlayerProperties PlayerProperties;
        
        public Transform PlayerCameraTransform;
        public BuildBlock LookAtObject;
        public BuildBlock ObjectInHands;
        public BuildBlock CachedPreviousUnderObject;
        public InitializableProperty<Vector3> InitialPosition;
        public InitializableProperty<Quaternion> InitialRotation;

        public UsedSpawnerProperties UsedSpawnerProperties;
    }

    public struct UsedSpawnerProperties
    {
        public AbstractSpawner Spawner;
        public bool WasApplied;
        public bool WasCancelled;
    }

    public struct PlayerProperties
    {
        public (float min, float max) GrabDistanceRange;
    }
}