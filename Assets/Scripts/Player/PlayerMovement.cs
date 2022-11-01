namespace Assets.Scripts.Player
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerMovement : MonoBehaviour
    {
        private readonly Vector3 FlatGroundNormal = Vector3.up;

        [Header("Transforms")]
        [SerializeField]
        private Transform _playerCameraTransform;

        [Header("Drag")]
        [SerializeField]
        private float _groundDrag = 7f;

        [SerializeField]
        private float _airDrag = 2f;

        [Header("Speed")]
        [SerializeField]
        private float _walkSpeed = 56f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _airSpeedMultiplier = 0.4f;

        [Header("Ground Detection")]
        [SerializeField]
        private float _groundCheckDistance = 0.2f;

        [SerializeField]
        private float _groundCheckRadiusReduction = 0.1f;

        [Range(0f, 1f)]
        [SerializeField]
        private float _slopeAngleMax = 0.385f;

        [SerializeField]
        private LayerMask _groundMask;

        private Rigidbody _playerRigidbody;
        private CapsuleCollider _playerCapsuleCollider;

        private bool _isGrounded;
        private Vector3 _groundNormal;

        private void Start()
        {
            _playerRigidbody = GetComponent<Rigidbody>();
            _playerCapsuleCollider = GetComponent<CapsuleCollider>();
        }

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

            _isGrounded = Physics.SphereCast(transform.position, radius, Vector3.down, out RaycastHit hit, distance, _groundMask) && hit.normal.y > slopeAngle;

            if (_isGrounded)
            {
                _groundNormal = hit.normal;
                _playerRigidbody.drag = _groundDrag;
            }
            else
            {
                _groundNormal = FlatGroundNormal;
                _playerRigidbody.drag = _airDrag;
            }
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

            if (!_isGrounded)
            {
                force *= _airSpeedMultiplier;
            }

            Move(force);
        }

        private void Move(Vector3 force) => _playerRigidbody.AddForce(force, ForceMode.Acceleration);

        private bool IsOnSlope => _groundNormal.y != FlatGroundNormal.y;

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
