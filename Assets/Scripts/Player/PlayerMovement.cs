namespace Assets.Scripts.Player
{
    using System;
    using UnityEngine;

    internal class PlayerMovement : MonoBehaviour
    {
        private readonly Vector3 FlatGroundNormal = Vector3.up;

        [Header("Geometry")]
        [SerializeField]
        private Transform _playerTransform;

        [SerializeField]
        private Transform _playerCameraTransform;

        [Header("Physics")]
        [SerializeField]
        private Rigidbody _playerRigidbody;

        [SerializeField]
        private CapsuleCollider _playerCapsuleCollider;

        [Header("Drag")]
        [SerializeField]
        private float _groundDrag;

        [SerializeField]
        private float _airDrag;

        [Header("Speed")]
        [SerializeField]
        private float _walkSpeed;

        [SerializeField]
        private float _acceleration;

        [SerializeField]
        [Range(0f, 1f)]
        private float _airSpeedMultiplier;

        [Header("Ground Detection")]
        [SerializeField]
        private float _groundCheckDistance;

        [SerializeField]
        private float _groundCheckRadiusReduction = 0.1f;

        [Range(0f, 1f)]
        [SerializeField]
        private float _slopeAngleMax = 0.385f;

        [SerializeField]
        private LayerMask _groundMask;

        private bool _isGrounded;
        private Vector3 _groundNormal;

        private void Update()
        {
            CheckGround();
        }

        private void FixedUpdate()
        {
            MoveOnSlope();
            Move();
        }

        private void CheckGround()
        {
            float distance = _playerCapsuleCollider.height * 0.5f + _groundCheckDistance;
            float radius = _playerCapsuleCollider.radius - _groundCheckRadiusReduction;
            float slopeAngle = 1f - _slopeAngleMax;
            IsGrounded = Physics.SphereCast(_playerTransform.position, radius, Vector3.down, out RaycastHit hit, distance, _groundMask) && hit.normal.y > slopeAngle;
            _groundNormal = IsGrounded ? hit.normal : FlatGroundNormal;
        }

        private void MoveOnSlope()
        {
            if (IsOnSlope)
            {
                Vector3 slipDirection = _groundNormal + Physics.gravity.normalized;
                Vector3 antiSlipVelocity = slipDirection * Physics.gravity.magnitude * -1f;
                Move(antiSlipVelocity);
            }
        }

        private void Move()
        {
            Vector3 force = MoveDirectionNormalized * _walkSpeed;

            if (!IsGrounded)
            {
                force *= _airSpeedMultiplier;
            }

            Move(force);
        }

        private void Move(Vector3 force) => _playerRigidbody.AddForce(force, ForceMode.Acceleration);

        private bool IsOnSlope => _groundNormal.y != FlatGroundNormal.y;

        private bool IsGrounded
        {
            get => _isGrounded;
            set
            {
                if (_isGrounded != value)
                {
                    _playerRigidbody.drag = value ? _groundDrag : _airDrag;
                    _isGrounded = value;
                }
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
