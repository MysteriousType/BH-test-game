namespace Assets.Scripts.Player
{
    using Assets.Scripts.Player.Data;
    using Mirror;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerMovement : NetworkBehaviour
    {
        public GameObject PlayerCamera;

        private readonly Vector3 FlatGroundNormal = Vector3.up;

        [Header("Transforms")]
        [SerializeField]
        private Transform _playerCameraTransform;

        [Header("Data")]
        [SerializeField]
        private PlayerData _playerData;

        private Rigidbody _playerRigidbody;
        private CapsuleCollider _playerCapsuleCollider;

        private bool _isGrounded;
        private Vector3 _groundNormal;

        private void Start()
        {
            _playerRigidbody = GetComponent<Rigidbody>();
            _playerCapsuleCollider = GetComponent<CapsuleCollider>();

            if (!isLocalPlayer)
            {
                return;
            }
        }

        private void Update()
        {
            if (!isLocalPlayer)
            {
                PlayerCamera.SetActive(false);
            }

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
            Vector3 force = MoveDirectionNormalized * _playerData.WalkSpeed;

            if (!_isGrounded)
            {
                force *= _playerData.AirSpeedMultiplier;
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
