using System;
using LaundryTest.PlayerModes;
using UnityEngine;
using PlayerInput = LaundryTest.InputSystem.PlayerInput;

namespace LaundryTest
{
    [RequireComponent(typeof(CharacterController))]
    public class GodPlayer : MonoBehaviour
    {
        [SerializeField] private Camera playerEyes;
        
        private const float SPEED = 5f;
        private float _scale = 0.3f;

        private DefaultControl _defaultControl;
        private CharacterController _characterController;
        private PlayerInput _input = new PlayerInput();
        private float _verticalRotation;

        private PlayerModeContext _playerModeContext = new PlayerModeContext();

        private AbstractMode _actualMode;
        private readonly AbstractMode _normalMode = new NormalMode();
        private readonly AbstractMode _buildModeCube = new BuildCube();
        private readonly AbstractMode _buildModeCircle = new BuildCircle();

        private EPlayerMode _actualModeType = EPlayerMode.Normal;
        
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _defaultControl = new DefaultControl();
            _defaultControl.Enable();
            _defaultControl.Movement.Enable();
            _defaultControl.GeneralActions.Enable();
            _defaultControl.NormalMode.Enable();

            _characterController = GetComponent<CharacterController>();

            _playerModeContext.PlayerProperties = new PlayerProperties() { GrabDistanceRange = (1, 6), };
            _playerModeContext.PlayerCameraTransform = playerEyes.transform;
            _playerModeContext.PlayerInput = _input;

            _actualMode = _normalMode;
        }

        private void Update()
        {
            _input.CancelPerformed.Value = _defaultControl.NormalMode.Cancel.ReadValueAsObject() != null;
            _input.InteractPerformed.Value = _defaultControl.NormalMode.Interact.ReadValueAsObject() != null;
            _input.Action1Performed.Value = _defaultControl.NormalMode.Action1.ReadValueAsObject() != null;
            _input.Action2Performed.Value = _defaultControl.NormalMode.Action2.ReadValueAsObject() != null;
            _input.Action3Performed.Value = _defaultControl.NormalMode.Action3.ReadValueAsObject() != null;
            _playerModeContext.PlayerInput = _input;

            var moveDirection = _defaultControl.Movement.Movement.ReadValue<Vector2>();
            if (moveDirection.x != 0 || moveDirection.y != 0)
            {
                MovePlayer(moveDirection);
            }

            var lookDelta = _defaultControl.Movement.LookRotationDelta.ReadValue<Vector2>();
            if (lookDelta.x != 0 || lookDelta.y != 0)
            {
                RotateLookDirection(lookDelta);
            }

            var nextModeType = _actualMode.Update(_playerModeContext);

            if (_actualModeType != nextModeType)
            {
                _actualModeType = nextModeType;
                _actualMode.Exit(_playerModeContext);
                _actualMode = _actualModeType switch
                {
                    EPlayerMode.Normal => _normalMode,
                    EPlayerMode.BuildCube => _buildModeCube,
                    EPlayerMode.BuildCircle => _buildModeCircle,
                    _ => throw new ArgumentOutOfRangeException()
                };
                _actualMode.Enter(_playerModeContext);
            }
        }

        private void MovePlayer(Vector2 movement)
        {
            Vector3 move = transform.right * movement.x + transform.forward * movement.y + Physics.gravity;
            _characterController.Move(move * (SPEED * Time.deltaTime));
        }

        private void RotateLookDirection(Vector2 lookDelta)
        {
            transform.Rotate(Vector3.up * (lookDelta.x * _scale));
            _verticalRotation -= lookDelta.y * _scale;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -90f, 90f);
            playerEyes.transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
        }
    }
}