namespace Assets.Scripts.Player
{
    using UnityEngine;

    public class PlayerCamera : MonoBehaviour
    {
        public float lookXLimit = 60.0f;
        public float lookSpeed = 2.0f;

        [Header("Transforms")]
        [SerializeField]
        private Transform _playerCameraParentTransform;

        private Vector2 _rotation;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _rotation.y = transform.eulerAngles.y;
        }

        private void Update()
        {
            _rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            _rotation.x += -1f * Input.GetAxis("Mouse Y") * lookSpeed;
            _rotation.x = Mathf.Clamp(_rotation.x, -lookXLimit, lookXLimit);
            _playerCameraParentTransform.localRotation = Quaternion.Euler(_rotation.x, 0, 0);
            transform.eulerAngles = new Vector2(0, _rotation.y);
        }
    }
}
