namespace Assets.Scripts.Player
{
    using Assets.Scripts.Player.Data;
    using Assets.Scripts.Scene;
    using Mirror;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Player : NetworkBehaviour
    {
        private const int ScoreToWin = 1;
        private const float InvincibilityDurationTimeMin = 0f;

        [Header("Score Info")]
        [SerializeField]
        public TextMesh _playerScoreText;

        [SerializeField]
        public Transform _playerScoreTextHolderTransform;

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

        [SyncVar(hook = nameof(OnScoreChanged))]
        private int _playerScore;

        private SceneWinnerText _sceneWinnerText;
        private PlayerMovement _playerMovement;
        private PlayerCamera _playerCamera;
        private Material _playerMeshMaterial;
        private float _invincibilityDurationTime;

        public bool ReceiveHit(float invincibilityEffectDuration)
        {
            if (IsInvincible)
            {
                return false;
            }

            _playerColor = Color.red;
            _invincibilityDurationTime = Time.time + invincibilityEffectDuration;
            return true;
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

            Vector3 scoreHolderLocalScale = _playerScoreTextHolderTransform.localScale;
            scoreHolderLocalScale.x *= -1f;
            _playerScoreTextHolderTransform.localScale = scoreHolderLocalScale;
        }

        private void Awake()
        {
            _sceneWinnerText = FindObjectOfType<SceneWinnerText>();
        }

        private void OnDestroy()
        {
            Destroy(_playerMeshMaterial);
        }

        private void Update()
        {
            if (IsInvincible && Time.time > _invincibilityDurationTime)
            {
                CmdExpireInvincibility();
            }

            if (isLocalPlayer)
            {
                _playerMovement.Update();
                _playerCamera?.Update();
            }
            else
            {
                _playerScoreTextHolderTransform.LookAt(Camera.main.transform);
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
            if (isLocalPlayer && _playerMovement.IsDashing && collision.gameObject.TryGetComponent(out Player hitPlayer))
            {
                CmdHit(hitPlayer, _playerData.DashInvincibilityTime);
            }
        }

        private bool IsInvincible => _invincibilityDurationTime != InvincibilityDurationTimeMin;

        [Command(requiresAuthority = false)]
        private void CmdHit(Player hitPlayer, float invincibilityEffectDuration)
        {
            if (hitPlayer.ReceiveHit(invincibilityEffectDuration))
            {
                TargetIncreaseScore();
            }
        }

        [TargetRpc]
        private void TargetIncreaseScore()
        {
            CmdIncreaseScore();
        }

        [Command(requiresAuthority = false)]
        private void CmdExpireInvincibility()
        {
            _playerColor = Color.white;
            _invincibilityDurationTime = InvincibilityDurationTimeMin;
        }

        [Command(requiresAuthority = false)]
        private void CmdIncreaseScore()
        {
            _playerScore++;

            if (_playerScore == ScoreToWin)
            {
                _sceneWinnerText.SetText($"{gameObject.name} is winner!");
            }
        }

        private void OnColorChanged(Color oldColor, Color newColor)
        {
            if (_playerMeshMaterial == null)
            {
                _playerMeshMaterial = new Material(_playerMeshRenderer.material);
            }

            _playerMeshMaterial.color = _playerColor;
            _playerMeshRenderer.material = _playerMeshMaterial;
        }

        private void OnScoreChanged(int oldScore, int newScore)
        {
            _playerScoreText.text = _playerScore.ToString();
        }
    }
}
