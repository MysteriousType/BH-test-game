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

        [SyncVar(hook = nameof(OnColorChanged))]
        private Color _playerColor;

        private PlayerMovement _playerMovement;
        private PlayerCamera _playerCamera;
        private Material _playerMeshMaterial;

        public void HitByDash()
        {
            _playerColor = Color.red;
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

        private void OnDestroy()
        {
            Destroy(_playerMeshMaterial);
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

        [ServerCallback]
        private void OnCollisionEnter(Collision collision)
        {
            if (_playerMovement.IsDashing && collision.gameObject.TryGetComponent(out Player player))
            {
                player.HitByDash();
            }
        }

        private void OnColorChanged(Color oldValue, Color newValue)
        {
            if (_playerMeshMaterial == null)
            {
                _playerMeshMaterial = _playerMeshRenderer.material;
            }

            _playerMeshMaterial.color = newValue;
        }
    }
}
