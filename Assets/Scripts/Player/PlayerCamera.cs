namespace Assets.Scripts.Player
{
    using UnityEngine;

    public class PlayerCamera : MonoBehaviour
    {
        [Header("Transforms")]
        [SerializeField]
        private Transform _playerCameraParentTransform;

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
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _rotation.y = transform.eulerAngles.y;
        }

        private void Update()
        {
            Vector2 mouseAxis = PlayerInput.MouseAxis;
            _rotation.y += mouseAxis.x * _lookSpeed;
            _rotation.x += mouseAxis.y * _lookSpeed * -1f;
            _rotation.x = Mathf.Clamp(_rotation.x, _lookXMin, _lookXMax);

            _playerCameraParentTransform.localRotation = Quaternion.Euler(_rotation.x, 0, 0);
            transform.eulerAngles = new Vector2(0, _rotation.y);
        }
    }
}
