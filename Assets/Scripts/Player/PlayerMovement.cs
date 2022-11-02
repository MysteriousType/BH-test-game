namespace Assets.Scripts.Player
{
    using Assets.Scripts.Player.Data;
    using UnityEngine;

    public class PlayerMovement
    {
        private readonly Vector3 FlatGroundNormal = Vector3.up;
        private readonly CapsuleCollider PlayerCapsuleCollider;
        private readonly Rigidbody PlayerRigidbody;
        private readonly PlayerData PlayerData;
        private readonly Transform PlayerCameraTransform;
        private readonly Transform PlayerTransform;

        private bool _isDashing;
        private bool _isGrounded;

        private Vector3 _groundNormal;
        private Vector3 _previousPosition;

        private float _dashingPassedDistance;

        public PlayerMovement(
            CapsuleCollider playerCapsuleCollider,
            Rigidbody playerRigidbody,
            PlayerData playerData,
            Transform playerCameraTransform,
            Transform playerTransform)
        {
            PlayerCapsuleCollider = playerCapsuleCollider;
            PlayerRigidbody = playerRigidbody;
            PlayerData = playerData;
            PlayerCameraTransform = playerCameraTransform;
            PlayerTransform = playerTransform;
        }

        public void Update()
        {
            CheckDashing();
            CheckGround();
        }

        public void FixedUpdate()
        {
            MoveOnSlope();
            Move();
        }

        private void CheckGround()
        {
            if (IsDashing)
            {
                return;
            }

            float distance = PlayerCapsuleCollider.height * 0.5f + PlayerData.GroundCheckDistance;
            float radius = PlayerCapsuleCollider.radius - PlayerData.GroundCheckRadiusReduction;
            float slopeAngle = 1f - PlayerData.SlopeAngleMax;

            _isGrounded = Physics.SphereCast(PlayerTransform.position, radius, Vector3.down, out RaycastHit hit, distance, PlayerData.GroundMask) && hit.normal.y > slopeAngle;

            if (_isGrounded)
            {
                _groundNormal = hit.normal;
                PlayerRigidbody.drag = PlayerData.GroundDrag;
            }
            else
            {
                _groundNormal = FlatGroundNormal;
                PlayerRigidbody.drag = PlayerData.AirDrag;
            }
        }

        private void CheckDashing()
        {
            if (PlayerInput.Dash)
            {
                IsDashing = true;
            }

            if (IsDashing)
            {
                float passedDistance = Vector3.Distance(PlayerTransform.position, _previousPosition);
                _dashingPassedDistance += passedDistance;

                if (_dashingPassedDistance >= PlayerData.DashDistance || passedDistance < 0.001f)
                {
                    IsDashing = false;
                }
            }

            _previousPosition = PlayerTransform.position;
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
                speed = PlayerData.DashSpeed;
            }
            else
            {
                speed = PlayerData.WalkSpeed;

                if (!_isGrounded)
                {
                    speed *= PlayerData.AirSpeedMultiplier;
                }
            }

            Vector3 force = movementDirection * speed;
            Move(force);
        }

        private void Move(Vector3 force) => PlayerRigidbody.AddForce(force, ForceMode.Acceleration);

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
                    PlayerRigidbody.velocity = Vector3.zero;
                }

                PlayerRigidbody.useGravity = !value;
                _isDashing = value;
            }
        }

        private Vector3 MoveDirectionNormalized
        {
            get
            {
                Vector2 axisInput = PlayerInput.AxisNormalized;
                Vector3 moveDirection = PlayerCameraTransform.right * axisInput.x + PlayerCameraTransform.forward * axisInput.y;
                moveDirection.y = 0f;

                return IsOnSlope
                    ? Vector3.ProjectOnPlane(moveDirection, _groundNormal).normalized
                    : moveDirection.normalized;
            }
        }
    }
}
