namespace Assets.Scripts.Player.Data
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/Player/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        [Header("Drag")]
        [SerializeField]
        private float _groundDrag = 7f;

        [SerializeField]
        private float _airDrag = 2f;

        [Header("Walk Speed")]
        [SerializeField]
        private float _walkSpeed = 56f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _airSpeedMultiplier = 0.4f;

        [Header("Dashing")]
        [SerializeField]
        private float _dashSpeed = 350f;

        [SerializeField]
        private float _dashDistance = 10f;

        [SerializeField]
        private float _dashInvincibilityTime = 3f;

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

        public float GroundDrag => _groundDrag;

        public float AirDrag => _airDrag;

        public float WalkSpeed => _walkSpeed;

        public float DashSpeed => _dashSpeed;

        public float DashDistance => _dashDistance;

        public float DashInvincibilityTime => _dashInvincibilityTime;

        public float AirSpeedMultiplier => _airSpeedMultiplier;

        public float GroundCheckDistance => _groundCheckDistance;

        public float GroundCheckRadiusReduction => _groundCheckRadiusReduction;

        public float SlopeAngleMax => _slopeAngleMax;

        public LayerMask GroundMask => _groundMask;
    }
}
