namespace Assets.Scripts.Player
{
    using Assets.Scripts.Player.Data;
    using UnityEngine;

    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField]
        private PlayerCameraData _playerCameraData;

        private Transform _playerTransform;
        private Transform _playerCameraHolderTransform;

        private Vector2 _playerRotation;
        private Vector3 _defaultCameraPosition;
        private float _defaultCameraPositionDistance;

        public void SetupCamera(Transform playerTransform, Transform playerCameraHolderTransform, PlayerCameraData playerCameraData)
        {
            _playerTransform = playerTransform;
            _playerCameraHolderTransform = playerCameraHolderTransform;
            _playerCameraData = playerCameraData;

            SetupTransform();
            SetupCursor();
            SetupDefaultValues();
        }

        private void Update()
        {
            Vector2 mouseAxis = PlayerInput.MouseAxis * _playerCameraData.LookSpeed;

            if (_playerCameraData.IsVerticallyInverted)
            {
                mouseAxis.y *= -1f;
            }

            _playerRotation.y += mouseAxis.x;
            _playerRotation.x += mouseAxis.y;
            _playerRotation.x = Mathf.Clamp(_playerRotation.x, _playerCameraData.LookXMin, _playerCameraData.LookXMax);

            _playerCameraHolderTransform.localRotation = Quaternion.Euler(_playerRotation.x, 0f, 0f);
            _playerTransform.eulerAngles = new Vector3(0f, _playerRotation.y, 0f);
        }

        private void LateUpdate()
        {
            Vector3 directionToCamera = transform.position - _playerCameraHolderTransform.position;

            transform.localPosition = Physics.SphereCast(_playerCameraHolderTransform.position, _playerCameraData.CollisionDetectionRadius, directionToCamera, out RaycastHit hit, _defaultCameraPositionDistance)
                ? Vector3.Normalize(_defaultCameraPosition) * (hit.distance - _playerCameraData.CollisionDetectionRadius)
                : Vector3.Lerp(transform.localPosition, _defaultCameraPosition, _playerCameraData.MoveSpeed * Time.deltaTime);
        }

        private void SetupTransform()
        {
            transform.parent = _playerCameraHolderTransform;
            transform.localPosition = _playerCameraData.LocalPosition;
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
