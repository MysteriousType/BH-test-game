namespace Assets.Scripts.Player.Data
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "PlayerCameraData", menuName = "ScriptableObjects/Player/PlayerCameraData")]
    public class PlayerCameraData : ScriptableObject
    {
        [Header("Camera Look")]
        [SerializeField]
        private float _lookXMin = -20f;

        [SerializeField]
        private float _lokXMax = 60f;

        [SerializeField]
        private float _lookSpeed = 2f;

        [SerializeField]
        private bool _isVerticallyInverted;

        [Header("Camera Collision Detection")]
        [SerializeField]
        private float _collisionDetectionRadius = 0.15f;

        [SerializeField]
        [Tooltip("How fast the camera should snap into the default position if there are no obstacles")]
        private float _moveSpeed = 15f;

        [Header("Position")]
        [SerializeField]
        [Tooltip("Position relative to the player camera holder position")]
        private Vector3 _localPosition;

        public float LookXMin => _lookXMin;

        public float LookXMax => _lokXMax;

        public float LookSpeed => _lookSpeed;

        public bool IsVerticallyInverted => _isVerticallyInverted;

        public float CollisionDetectionRadius => _collisionDetectionRadius;

        public float MoveSpeed => _moveSpeed;

        public Vector3 LocalPosition => _localPosition;

    }
}
