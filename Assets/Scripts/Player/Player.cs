namespace Assets.Scripts.Player
{
    using Assets.Scripts.Player.Data;
    using Mirror;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Player : NetworkBehaviour
    {
        [Header("Mesh")]
        [SerializeField]
        private MeshRenderer _playerMeshRenderer;

        [Header("Transforms")]
        [SerializeField]
        private Transform _playerCameraHolderTransform;

        [Header("Data")]
        [SerializeField]
        private PlayerData _playerData;

        [SerializeField]
        private PlayerCameraData _playerCameraData;

        private PlayerMovement _playerMovement;
        private PlayerCamera _playerCamera;

        public void HitByDash()
        {
            _playerMeshRenderer.material.color = Color.red;
        }

        public override void OnStartLocalPlayer()
        {
            Camera camera = Camera.main;

            if (camera == null)
            {
                Debug.LogError($"MainCamera cannot be found!");
                return;
            }

            _playerCamera = new PlayerCamera(_playerCameraData, _playerCameraHolderTransform, transform, camera.transform);
        }

        private void Start()
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

            _playerMovement = new PlayerMovement(capsuleCollider, rigidbody, _playerData, _playerCameraHolderTransform, transform);
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                _playerMovement.Update();
            }

            _playerCamera?.Update();
        }

        private void FixedUpdate()
        {
            if (isLocalPlayer)
            {
                _playerMovement.FixedUpdate();
            }
        }

        private void LateUpdate()
        {
            _playerCamera?.LateUpdate();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Player player))
            {
                player.HitByDash();
            }
        }
    }
}
