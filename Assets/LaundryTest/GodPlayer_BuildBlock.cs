using System;
using Assets.LaundryTest.Buildings.Blocks;
using LaundryTest.NewWay;
using LaundryTest.RaycastScanners;
using UnityEditor.PackageManager;
using UnityEngine;

namespace LaundryTest
{
    [RequireComponent(typeof(CharacterController))]
    public class GodPlayer_BuildBlock : MonoBehaviour
    {
        private const float SPEED = 5f;
        private float _scale = 0.3f;

        [SerializeField] private Camera playerEyes;
        [SerializeField] private float grabDistance = 2.5f;
        [SerializeField] private string layerName;
        private DefaultControl _defaultControl;
        private CharacterController _characterController;
        private float _verticalRotation;

        [SerializeField] private BuildBlock _lookAtObject;
        [SerializeField] private BuildBlock _objectInHands;

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
            Gizmos.DrawLine(playerEyes.transform.position, playerEyes.transform.position + playerEyes.transform.forward*grabDistance);
            Gizmos.DrawLine(_boxHitInfo.point, _boxHitInfo.point + _boxHitInfo.normal);
        }

        private void SetObjectsInHand(BuildBlock val)
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
                if (Physics.Raycast(
                    playerEyes.transform.position,
                    playerEyes.transform.forward,
                    out var hitInfo,
                    grabDistance,
                    ~LayerMask.GetMask(GameplayLayers.GrabableLayerName)))
                {
                    bool isNotTopSurface = hitInfo.normal != Vector3.up;

                    _targetPosition = hitInfo.point;
                    _objectInHands.MoveObject(_targetPosition, isNotTopSurface); // correction movement
                    _isCastSucced = true;
                    //_objectInHands.RotateObject(Quaternion.LookRotation(_objectInHands.transform.forward, hitInfo.normal), isNotTopSurface);
                }
                else
                {
                    _objectInHands.MoveObject(playerEyes.transform.position + playerEyes.transform.forward * _distanceToObject);
                    _isCastSucced = false;
                }
            }
            else
            {
                if (Physics.Raycast(
                    playerEyes.transform.position,
                    playerEyes.transform.forward,
                    out var hitInfo,
                    grabDistance,
                    LayerMask.GetMask(GameplayLayers.GrabableLayerName)))
                {
                    if (hitInfo.transform.TryGetComponent<BuildBlock>(out var block))
                    {
                        if (_lookAtObject != block)
                        {
                            _lookAtObject = block;
                        }

                        Debug.Log(_lookAtObject.name);
                    }
                }
                else
                {
                    _lookAtObject = null;
                }
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
                        //_lookAtObject.SetCollisionsEnabled(false);
                        SetObjectsInHand(_lookAtObject);
                        _distanceToObject = Vector3.Distance(playerEyes.transform.position, _initialPosition);
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            //_actualScanner.ScanForObject<BuildObject>();
            
            

            /*if(_objectInHands != null)
            {
                if (_isCastSucced)
                {
                    _objectInHands.MoveObject(_targetPosition);
                }
                else
                {
                    
                }
            }*/
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