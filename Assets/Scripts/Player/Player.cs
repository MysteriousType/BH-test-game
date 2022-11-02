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

        private PlayerMovement _playerMovement;

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
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

            _playerMovement = new PlayerMovement(capsuleCollider, rigidbody, _playerData, _playerCameraTransform, transform);
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                _playerMovement.Update();
            }
        }

        private void FixedUpdate()
        {
            if (isLocalPlayer)
            {
                _playerMovement.FixedUpdate();
            }
        }
    }
}
