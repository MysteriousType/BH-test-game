namespace Assets.Scripts.Player
{
    using Assets.Scripts.Player.Data;
    using Mirror;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Player : NetworkBehaviour
    {
        private const float DashHitDurationTimeMin = 0f;

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

        private float _dashHitDurationTime;

        [Command(requiresAuthority = false)]
        public void CmdHitByDash()
        {
            _playerColor = Color.red;
            _dashHitDurationTime = Time.time + _playerData.DashHitDurationTime;
        }

        public override void OnStartLocalPlayer()
        {
            Camera camera = Camera.main;

            if (camera == null)
            {
                Debug.LogError($"MainCamera cannot be found!");
                return;
            }

            Rigidbody rigidbody = GetComponent<Rigidbody>();
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

            _playerMovement = new PlayerMovement(capsuleCollider, rigidbody, _playerData, _playerCameraHolderTransform, transform);
            _playerCamera = new PlayerCamera(_playerCameraData, _playerCameraHolderTransform, transform, camera.transform);
        }

        private void OnDestroy()
        {
            Destroy(_playerMeshMaterial);
        }

        private void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            _playerMovement.Update();
            _playerCamera?.Update();

            if (_dashHitDurationTime > DashHitDurationTimeMin && Time.time > _dashHitDurationTime)
            {
                CmdDashHitExpired();
            }
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
            if (isLocalPlayer && _playerMovement.IsDashing && collision.gameObject.TryGetComponent(out Player player))
            {
                player.CmdHitByDash();
            }
        }

        [Command]
        private void CmdDashHitExpired()
        {
            _playerColor = Color.white;
            _dashHitDurationTime = DashHitDurationTimeMin;
        }

        private void OnColorChanged(Color oldValue, Color newValue)
        {
            if (_playerMeshMaterial == null)
            {
                _playerMeshMaterial = new Material(_playerMeshRenderer.material);
            }

            _playerMeshMaterial.color = _playerColor;
            _playerMeshRenderer.material = _playerMeshMaterial;
        }
    }
}
