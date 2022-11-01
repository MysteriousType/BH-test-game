namespace Assets.Scripts.Player
{
    using UnityEngine;

    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Transforms")]
        [SerializeField]
        private Transform _playerTransform;

        [SerializeField]
        private Transform _playerCameraHolderTransform;

        [Header("Look Settings")]
        [SerializeField]
        private float _lookXMin = -20f;

        [SerializeField]
        private float _lookXMax = 60f;

        [SerializeField]
        private float _lookSpeed = 2f;

        [Header("Collision Detection Settings")]
        [SerializeField]
        private float _collisionDetectionRadius = 0.15f;

        [SerializeField]
        [Tooltip("How fast the camera should snap into the default position if there are no obstacles")]
        private float _cameraMoveSpeed = 15f;

        private Vector2 _playerRotation;
        private Vector3 _defaultCameraPosition;
        private float _defaultCameraPositionDistance;

        private void Start()
        {
            SetupDefaultValues();
            SetupCursor();
        }

        private void Update()
        {
            Vector2 mouseAxis = PlayerInput.MouseAxis;
            _playerRotation.y += mouseAxis.x * _lookSpeed;
            _playerRotation.x += mouseAxis.y * _lookSpeed * -1f;
            _playerRotation.x = Mathf.Clamp(_playerRotation.x, _lookXMin, _lookXMax);

            _playerCameraHolderTransform.localRotation = Quaternion.Euler(_playerRotation.x, 0f, 0f);
            _playerTransform.eulerAngles = new Vector3(0f, _playerRotation.y, 0f);
        }

        private void LateUpdate()
        {
            Vector3 directionToCamera = transform.position - _playerCameraHolderTransform.position;

            if (Physics.SphereCast(_playerCameraHolderTransform.position, _collisionDetectionRadius, directionToCamera, out RaycastHit hit, _defaultCameraPositionDistance))
            {
                float distance = hit.distance - _collisionDetectionRadius;
                transform.localPosition = Vector3.Normalize(_defaultCameraPosition) * distance;
                return;
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, _defaultCameraPosition, _cameraMoveSpeed * Time.deltaTime);
        }

        private void SetupDefaultValues()
        {
            _playerRotation.y = _playerTransform.eulerAngles.y;
            _defaultCameraPosition = transform.localPosition;
            _defaultCameraPositionDistance = Vector3.Distance(transform.position, _playerCameraHolderTransform.position);
        }

        private void SetupCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
