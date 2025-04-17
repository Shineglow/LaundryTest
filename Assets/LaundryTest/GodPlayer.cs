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
        private float _verticalRotation;
        
        // [SerializeField] private float grabDistance = 2.5f;

        // [SerializeField] private BuildBlock _lookAtObject = null;
        // [SerializeField] private BuildBlock _objectInHands = null;

        // private bool IsObjectInHands => _objectInHands != null;
        // private Vector3 _initialPosition;
        // private Quaternion _initialRotation;
        // private float _distanceToObject;
        //
        // private RaycastHit _boxHitInfo;
        // private Vector3 _targetPosition;

        // private void SetObjectsInHand(BuildBlock val)
        // {
        //     if (!_objectInHands.IsUnityNull())
        //     {
        //         _objectInHands.SetGrabbed(false);
        //     }
        //     _objectInHands = val;
        //     if (!_objectInHands.IsUnityNull())
        //     {
        //         _objectInHands.SetGrabbed(val != null);
        //     }
        // }

        private PlayerModeContext _playerModeContext = new PlayerModeContext();

        private AbstractMode _actualMode;
        private readonly AbstractMode _normalMode = new NormalMode();
        private readonly AbstractMode _buildMode = new BuildMode();

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

            _actualMode = _normalMode;
        }

        private void Update()
        {
            PlayerInput input = new PlayerInput()
            {
                CancelPerformed = _defaultControl.NormalMode.Cancel.ReadValueAsObject() != null,
                InteractPerformed = _defaultControl.NormalMode.Interact.ReadValueAsObject() != null,
            };
            _playerModeContext.PlayerInput = input;
            
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

            var nextMode = _actualMode.Update(_playerModeContext);

            if (_actualModeType != nextMode)
            {
                _actualModeType = nextMode;
                _actualMode.Exit(_playerModeContext);
                _actualMode = _actualModeType switch
                {
                    EPlayerMode.Normal => _normalMode,
                    EPlayerMode.Build => _buildMode,
                    _ => throw new ArgumentOutOfRangeException()
                };
                _actualMode.Enter(_playerModeContext);
            }

            // var cameraTransform = playerEyes.transform;
            //
            // if (IsObjectInHands)
            // {
            //     var hitResult = _objectInHands.CastForm(cameraTransform.position, cameraTransform.forward,
            //         grabDistance, out var hitInfo, ~LayerMask.GetMask(GameplayLayers.GrabbedLayerName, GameplayLayers.Player, GameplayLayers.SurfaceDetector));
            //     var distance = hitResult ? hitInfo.distance : _distanceToObject;
            //     _objectInHands.MoveObject(cameraTransform.position + cameraTransform.forward * distance);
            // }
            // else
            // {
            //     if (Physics.Raycast(
            //         cameraTransform.position,
            //         cameraTransform.forward,
            //         out var hitInfo,
            //         grabDistance,
            //         LayerMask.GetMask(GameplayLayers.GrabableLayerName)))
            //     {
            //         if (hitInfo.transform.TryGetComponent<BuildBlock>(out var block))
            //         {
            //             if (_lookAtObject != block)
            //             {
            //                 _lookAtObject = block;
            //             }
            //         }
            //     }
            //     else
            //     {
            //         _lookAtObject = null;
            //     }
            // }
            //
            // if (IsObjectInHands)
            // {
            //     if (_defaultControl.NormalMode.Interact.ReadValueAsObject() != null)
            //     {
            //         
            //     }
            //     else if (_defaultControl.NormalMode.Cancel.ReadValueAsObject() != null) /* cancel performed */
            //     {
            //         _objectInHands.MoveObject(_initialPosition);
            //         _objectInHands.RotateObject(_initialRotation);
            //         // TODO: change layer to GRABBED
            //         // TODO: configure layer to grabbed object to not collide with player but collide with others
            //         SetObjectsInHand(null);
            //         _distanceToObject = 0f;
            //     }
            // }
            // else 
            // {
            //     if (_lookAtObject != null) /* if player look at grabable object */
            //     {
            //         if (_defaultControl.NormalMode.Interact.ReadValueAsObject() != null) /* interact performed */
            //         {
            //             // TODO: switch to build mode
            //             // TODO: lock object on cursor
            //             _initialPosition = _lookAtObject.transform.position;
            //             _initialRotation = _lookAtObject.transform.rotation;
            //             // TODO: change layer to GRABABLE
            //             SetObjectsInHand(_lookAtObject);
            //             _distanceToObject = Vector3.Distance(cameraTransform.position, _initialPosition);
            //         }
            //     }
            // }
        }

        // private void Rotate(RaycastHit hitInfo)
        // {
        //     var inHandsTransform = _objectInHands.transform;
        //     var inHandsForward = inHandsTransform.forward;
        //     var hitNormal = hitInfo.normal;
        //     var angle = Vector3.Angle(inHandsForward.MultiplyComponents(Vector3Utils.nonUp),
        //         hitNormal.MultiplyComponents(Vector3Utils.nonUp));
        //     var rotation = Quaternion.Euler(0, angle, 0);
        //     inHandsTransform.rotation *= rotation;
        // }

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