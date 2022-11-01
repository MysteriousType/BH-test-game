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
        private float _lookXMin = -60f;

        [SerializeField]
        private float _lookXMax = 60f;

        [SerializeField]
        private float _lookSpeed = 2f;

        private Vector2 _rotation;

        private void Start()
        {
            SetupCursor();
            _rotation.y = _playerTransform.eulerAngles.y;
        }

        private void Update()
        {
            Vector2 mouseAxis = PlayerInput.MouseAxis;
            _rotation.y += mouseAxis.x * _lookSpeed;
            _rotation.x += mouseAxis.y * _lookSpeed * -1f;
            _rotation.x = Mathf.Clamp(_rotation.x, _lookXMin, _lookXMax);

            _playerCameraHolderTransform.localRotation = Quaternion.Euler(_rotation.x, 0f, 0f);
            _playerTransform.eulerAngles = new Vector3(0f, _rotation.y, 0f);
        }

        private void SetupCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
