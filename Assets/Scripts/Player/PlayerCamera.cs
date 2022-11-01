namespace Assets.Scripts.Player
{
    using UnityEngine;

    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : MonoBehaviour
    {
        public float collisionOffset = 0.3f; //To prevent Camera from clipping through Objects
        public float cameraSpeed = 15f; //How fast the Camera should snap into position if there are no obstacles

        Vector3 defaultPos;
        Vector3 directionNormalized;
        Transform parentTransform;
        float defaultDistance;

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

            defaultPos = transform.localPosition;
            directionNormalized = defaultPos.normalized;
            parentTransform = transform.parent;
            defaultDistance = Vector3.Distance(defaultPos, Vector3.zero);
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
            Vector3 currentPos = defaultPos;
            Vector3 dirTmp = parentTransform.TransformPoint(defaultPos) - _playerCameraHolderTransform.position;

            if (Physics.SphereCast(_playerCameraHolderTransform.position, collisionOffset, dirTmp, out RaycastHit hit, defaultDistance))
            {
                currentPos = directionNormalized * (hit.distance - collisionOffset);
                transform.localPosition = currentPos;
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, currentPos, Time.deltaTime * cameraSpeed);
            }
        }

        private void SetupCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
