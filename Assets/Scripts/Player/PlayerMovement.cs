namespace Assets.Scripts.Player
{
    using Assets.Scripts.Player.Data;
    using Mirror;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerMovement : NetworkBehaviour
    {
        private readonly Vector3 FlatGroundNormal = Vector3.up;

        [Header("Transforms")]
        [SerializeField]
        private Transform _playerCameraTransform;

        [SerializeField]
        private Transform _playerCameraHolderTransform;

        [Header("Data")]
        [SerializeField]
        private PlayerData _playerData;

        [SerializeField]
        private PlayerCameraData _playerCameraData;

        private Rigidbody _playerRigidbody;
        private CapsuleCollider _playerCapsuleCollider;

        private bool _isDashing;
        private bool _isGrounded;
        private Vector3 _groundNormal;
        private Vector3 _previousPosition;
        private float _dashingPassedDistance;

        public override void OnStartLocalPlayer()
        {
            Camera camera = Camera.main;

            if (camera == null)
            {
                Debug.LogError($"MainCamera cannot be found!");
                return;
            }

            if (!camera.TryGetComponent(out PlayerCamera playerCamera))
            {
                Debug.LogError($"{nameof(PlayerCamera)} component isn't set to the MainCamera!");
                return;
            }

            playerCamera.SetupCamera(transform, _playerCameraHolderTransform, _playerCameraData);
        }

        private void Start()
        {
            _playerRigidbody = GetComponent<Rigidbody>();
            _playerCapsuleCollider = GetComponent<CapsuleCollider>();
        }

        private void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            CheckDashing();
            CheckGround();
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            MoveOnSlope();
            Move();
        }

        private void CheckGround()
        {
            if (IsDashing)
            {
                return;
            }

            float distance = _playerCapsuleCollider.height * 0.5f + _playerData.GroundCheckDistance;
            float radius = _playerCapsuleCollider.radius - _playerData.GroundCheckRadiusReduction;
            float slopeAngle = 1f - _playerData.SlopeAngleMax;

            _isGrounded = Physics.SphereCast(transform.position, radius, Vector3.down, out RaycastHit hit, distance, _playerData.GroundMask) && hit.normal.y > slopeAngle;

            if (_isGrounded)
            {
                _groundNormal = hit.normal;
                _playerRigidbody.drag = _playerData.GroundDrag;
            }
            else
            {
                _groundNormal = FlatGroundNormal;
                _playerRigidbody.drag = _playerData.AirDrag;
            }
        }

        private void CheckDashing()
        {
            if (!IsDashing && PlayerInput.Dash)
            {
                IsDashing = true;
            }

            if (IsDashing)
            {
                float passedDistance = Vector3.Distance(transform.position, _previousPosition);
                _dashingPassedDistance += passedDistance;

                if (_dashingPassedDistance >= _playerData.DashDistance || passedDistance < 0.001f)
                {
                    IsDashing = false;
                }
            }

            _previousPosition = transform.position;
        }

        private void MoveOnSlope()
        {
            if (IsOnSlope && !IsDashing)
            {
                Vector3 slipDirection = _groundNormal + Physics.gravity.normalized;
                Vector3 antiSlipVelocity = slipDirection * Physics.gravity.magnitude * -1f;
                Move(antiSlipVelocity);
            }
        }

        private void Move()
        {
            float speed;
            Vector3 movementDirection = MoveDirectionNormalized;

            if (IsDashing)
            {
                speed = _playerData.DashSpeed;
            }
            else
            {
                speed = _playerData.WalkSpeed;

                if (!_isGrounded)
                {
                    speed *= _playerData.AirSpeedMultiplier;
                }
            }

            Vector3 force = movementDirection * speed;
            Move(force);
        }

        private void Move(Vector3 force) => _playerRigidbody.AddForce(force, ForceMode.Acceleration);

        private bool IsOnSlope => _groundNormal.y != FlatGroundNormal.y;

        private bool IsDashing
        {
            get => _isDashing;
            set
            {
                if (_isDashing == value)
                {
                    return;
                }

                if (!value)
                {
                    _dashingPassedDistance = 0f;
                    _playerRigidbody.velocity = Vector3.zero;
                }

                _playerRigidbody.useGravity = !value;
                _isDashing = value;
            }
        }

        private Vector3 MoveDirectionNormalized
        {
            get
            {
                Vector2 axisInput = PlayerInput.AxisNormalized;
                Vector3 moveDirection = _playerCameraTransform.right * axisInput.x + _playerCameraTransform.forward * axisInput.y;
                moveDirection.y = 0f;

                return IsOnSlope
                    ? Vector3.ProjectOnPlane(moveDirection, _groundNormal).normalized
                    : moveDirection.normalized;
            }
        }
    }
}
