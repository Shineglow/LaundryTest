using System;
using LaundryTest.NewWay;
using LaundryTest.RaycastScanners;
using UnityEngine;

namespace LaundryTest
{
    public class GodPlayer : MonoBehaviour
    {
        private const float SPEED = 5f;
        private float _scale = 0.3f;

        [SerializeField] private Camera playerEyes;
        [SerializeField] private float grabDistance = 2.5f;
        [SerializeField] private string layerName;
        private DefaultControl _defaultControl;
        private CharacterController _characterController;
        private float _verticalRotation;

        private BuildObject _lookAtObject;
        private BuildObject _objectInHands;
        
        private bool IsObjectInHands => _objectInHands != null;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;
        private float _distanceToObject;

        private IRaycastScanner _normalScanner;
        private IRaycastScanner _buildScanner;
        private IRaycastScanner _actualScanner;
        private RaycastHit _boxHitInfo;
        private Vector3 _targetPosition;
        private bool _isCastSucced;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_boxHitInfo.point, _boxHitInfo.point + _boxHitInfo.normal);
        }

        private void SetObjectsInHand(BuildObject val)
        {
            _objectInHands = val;
            _actualScanner = val == null ? _normalScanner : _buildScanner;
        }

        private void Awake()
        {
            _normalScanner = new DefaultScanner(playerEyes.transform, grabDistance, layerName, "Grabable");
            _buildScanner = new DefaultScanner(playerEyes.transform, grabDistance, layerName, "Surface");
            _actualScanner = _normalScanner;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _defaultControl = new DefaultControl();
            _defaultControl.Enable();
            _defaultControl.Movement.Enable();
            _defaultControl.GeneralActions.Enable();
            _defaultControl.NormalMode.Enable();

            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
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

            if (IsObjectInHands)
            {
                if (_defaultControl.NormalMode.Interact.ReadValueAsObject() != null)
                {
                    
                }
                else if (_defaultControl.NormalMode.Cancel.ReadValueAsObject() != null) /* cancel performed */
                {
                    _lookAtObject.transform.position = _initialPosition;
                    _lookAtObject.transform.rotation = _initialRotation;
                    _lookAtObject.SetCollisionsEnabled(true);
                    _distanceToObject = 0f;
                }
            }
            else 
            {
                if (_lookAtObject != null) /* if player look at grabable object */
                {
                    if (_defaultControl.NormalMode.Interact.ReadValueAsObject() != null) /* interact performed */
                    {
                        // TODO: switch to build mode
                        // TODO: lock object on cursor
                        _initialPosition = _lookAtObject.transform.position;
                        _initialRotation = _lookAtObject.transform.rotation;
                        _lookAtObject.SetCollisionsEnabled(false);
                        SetObjectsInHand(_lookAtObject);
                        _distanceToObject = Vector3.Distance(playerEyes.transform.position, _initialPosition);
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            _actualScanner.ScanForObject<BuildObject>();
            
            if (IsObjectInHands)
            {
                if (EasyRaycast(out var hitInfo, grabDistance, GameplayLayers.TopLayerName))
                {
                    _targetPosition = hitInfo.point + hitInfo.normal * _objectInHands.Size.y / 2;
                    _objectInHands.transform.rotation.SetLookRotation(_objectInHands.transform.forward, hitInfo.normal);
                    _isCastSucced = true;

                    // if (hitInfo.transform.TryGetComponent<BuildObject>(out var block) &&
                    //     _lookAtObject != block)
                    // {
                    //     _lookAtObject = block;
                    //     Debug.Log(_lookAtObject.name);
                    // }
                }
                else
                {
                    _isCastSucced = false;
                }
            }
            else
            {
                if (EasyRaycast(out var hitInfo, grabDistance, GameplayLayers.GrabableLayerName))
                {
                    if (hitInfo.transform.TryGetComponent<BuildObject>(out var block) &&
                        _lookAtObject != block)
                    {
                        _lookAtObject = block;
                        Debug.Log(_lookAtObject.name);
                    }
                }
                else
                {
                    _lookAtObject = null;
                }
            }
        }

        private void Rotate(RaycastHit hitInfo)
        {
            var inHandsTransform = _objectInHands.transform;
            var inHandsForward = inHandsTransform.forward;
            var hitNormal = hitInfo.normal;
            var angle = Vector3.Angle(inHandsForward.MultiplyComponents(Vector3Utils.nonUp),
                hitNormal.MultiplyComponents(Vector3Utils.nonUp));
            var rotation = Quaternion.Euler(0, angle, 0);
            inHandsTransform.rotation *= rotation;
        }

        private void MovePlayer(Vector2 movement)
        {
            Vector3 move = transform.right * movement.x + transform.forward * movement.y;
            _characterController.Move(move * (SPEED * Time.deltaTime));
            
            MoveHandedObject();
        }

        private void RotateLookDirection(Vector2 lookDelta)
        {
            transform.Rotate(Vector3.up * (lookDelta.x * _scale));
            _verticalRotation -= lookDelta.y * _scale;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -90f, 90f);
            playerEyes.transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);

            MoveHandedObject();
        }

        private void MoveHandedObject()
        {
            if (IsObjectInHands)
            {
                if (_isCastSucced)
                {
                    _objectInHands.transform.position = _targetPosition;
                }
                else
                {
                    _objectInHands.transform.position = playerEyes.transform.position + playerEyes.transform.forward * _distanceToObject;
                }
            }
        }
        
        private bool EasyRaycast(out RaycastHit hitInfo, float maxDistance, string layerName)
        {
            Transform cameraTransform = playerEyes.transform;
            return Physics.Raycast(
                cameraTransform.position,
                cameraTransform.forward,
                out hitInfo,
                maxDistance,
                LayerMask.GetMask(layerName));
        }
    }
}