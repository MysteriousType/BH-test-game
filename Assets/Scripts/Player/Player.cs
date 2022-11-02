namespace Assets.Scripts.Player
{
    using Assets.Scripts.Player.Data;
    using Mirror;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Player : NetworkBehaviour
    {
        [Header("Transforms")]
        [SerializeField]
        private Transform _playerCameraTransform;

        [SerializeField]
        private Transform _playerCameraHolderTransform;

        [Header("Data")]
        [SerializeField]
        private PlayerData _playerData;

        [SerializeField]
        private PlayerCameraData _playerCameraData;

        private Rigidbody _playerRigidbody;
        private CapsuleCollider _playerCapsuleCollider;

        public override void OnStartLocalPlayer()
        {
            Camera camera = Camera.main;

            if (camera == null)
            {
                Debug.LogError($"MainCamera cannot be found!");
                return;
            }

            if (!camera.TryGetComponent(out PlayerCamera playerCamera))
            {
                Debug.LogError($"{nameof(PlayerCamera)} component isn't set to the MainCamera!");
                return;
            }

            playerCamera.SetupCamera(transform, _playerCameraHolderTransform, _playerCameraData);
        }

        private void Start()
        {
            _playerRigidbody = GetComponent<Rigidbody>();
            _playerCapsuleCollider = GetComponent<CapsuleCollider>();
        }
    }
}
