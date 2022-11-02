﻿namespace Assets.Scripts.Player
{
    using Assets.Scripts.Player.Data;
    using Mirror;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Player : NetworkBehaviour
    {
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
        private float _playerScore;

        private PlayerMovement _playerMovement;
        private PlayerCamera _playerCamera;
        private Material _playerMeshMaterial;
        private float _invincibilityDurationTime;

        [Command(requiresAuthority = false)]
        public void CmdHit(float invincibilityEffectDuration)
        {
            _playerColor = Color.red;
            _invincibilityDurationTime = Time.time + invincibilityEffectDuration;
        }

        public bool IsInvincible => _invincibilityDurationTime != InvincibilityDurationTimeMin;

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
            bool canDash = isLocalPlayer && _playerMovement.IsDashing;

            if (canDash && collision.gameObject.TryGetComponent(out Player hitPlayer) && !hitPlayer.IsInvincible)
            {
                hitPlayer.CmdHit(_playerData.DashInvincibilityTime);
                CmdIncreaseScore();
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdExpireInvincibility()
        {
            _playerColor = Color.white;
            _invincibilityDurationTime = InvincibilityDurationTimeMin;
        }

        [Command]
        private void CmdIncreaseScore()
        {
            _playerScore++;
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

        private void OnScoreChanged(float oldScore, float newScore)
        {
            _playerScoreText.text = _playerScore.ToString();
        }
    }
}
