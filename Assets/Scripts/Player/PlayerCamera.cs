namespace Assets.Scripts.Player
{
    using Assets.Scripts.Player.Data;
    using UnityEngine;

    public class PlayerCamera
    {
        private readonly PlayerCameraData PlayerCameraData;
        private readonly Transform PlayerCameraHolderTransform;
        private readonly Transform PlayerTransform;
        private readonly Transform PlayerCameraTransform;
        private readonly Vector3 DefaultCameraPosition;
        private readonly float DefaultCameraPositionDistance;

        private Vector2 _playerRotation;

        public PlayerCamera(PlayerCameraData playerCameraData, Transform playerCameraHolderTransform, Transform playerTransform, Transform playerCameraTransform)
        {
            PlayerCameraData = playerCameraData;
            PlayerCameraHolderTransform = playerCameraHolderTransform;
            PlayerTransform = playerTransform;

            PlayerCameraTransform = playerCameraTransform;
            PlayerCameraTransform.parent = PlayerCameraHolderTransform;
            PlayerCameraTransform.localPosition = PlayerCameraData.LocalPosition;

            DefaultCameraPosition = playerCameraTransform.localPosition;
            DefaultCameraPositionDistance = Vector3.Distance(playerCameraTransform.position, PlayerCameraHolderTransform.position);

            _playerRotation.y = PlayerTransform.eulerAngles.y;

            SetupCursor();
        }

        public void Update()
        {
            Vector2 mouseAxis = PlayerInput.MouseAxis * PlayerCameraData.LookSpeed;

            if (PlayerCameraData.IsVerticallyInverted)
            {
                mouseAxis.y *= -1f;
            }

            _playerRotation.y += mouseAxis.x;
            _playerRotation.x += mouseAxis.y;
            _playerRotation.x = Mathf.Clamp(_playerRotation.x, PlayerCameraData.LookXMin, PlayerCameraData.LookXMax);

            PlayerCameraHolderTransform.localRotation = Quaternion.Euler(_playerRotation.x, 0f, 0f);
            PlayerTransform.eulerAngles = new Vector3(0f, _playerRotation.y, 0f);
        }

        public void LateUpdate()
        {
            Vector3 directionToCamera = PlayerCameraTransform.position - PlayerCameraHolderTransform.position;

            PlayerCameraTransform.localPosition = Physics.SphereCast(PlayerCameraHolderTransform.position, PlayerCameraData.CollisionDetectionRadius, directionToCamera, out RaycastHit hit, DefaultCameraPositionDistance)
                ? Vector3.Normalize(DefaultCameraPosition) * (hit.distance - PlayerCameraData.CollisionDetectionRadius)
                : Vector3.Lerp(PlayerCameraTransform.localPosition, DefaultCameraPosition, PlayerCameraData.MoveSpeed * Time.deltaTime);
        }

        private void SetupCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
