namespace Assets.Scripts.Player
{
    using UnityEngine;

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

        private Vector2 _playerRotation;

        private void Start()
        {
            SetupCursor();
            _playerRotation.y = _playerTransform.eulerAngles.y;
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

        private void SetupCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
