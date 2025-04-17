using LaundryTest.Buildings.Blocks;
using LaundryTest.InputSystem;
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
        public InitializableProperty<Vector3> InitialPosition;
        public InitializableProperty<Quaternion> InitialRotation;
    }

    public struct PlayerProperties
    {
        public (float min, float max) GrabDistanceRange;
    }
}