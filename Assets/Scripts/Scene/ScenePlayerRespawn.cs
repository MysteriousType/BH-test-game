namespace Assets.Scripts.Scene
{
    using Assets.Scripts.Player;
    using Mirror;
    using UnityEngine;

    public class ScenePlayerRespawn : NetworkBehaviour
    {
        private const float RespawnDelayDurationTimeMin = 0f;
        private readonly SyncList<Player> Players = new SyncList<Player>();

        [Header("Respawn Settings")]
        [SerializeField]
        private float _respawnDelayTime = 5f;

        private float _respawnDelayDurationTime;

        [Command(requiresAuthority = false)]
        public void CmdAddPlayer(Player player) => Players.Add(player);

        [Command(requiresAuthority = false)]
        public void CmdRemovePlayer(Player player) => Players.Remove(player);

        [Command(requiresAuthority = false)]
        public void CmdDelayedRespawnAll()
        {
            if (!IsInRespawnMode)
            {
                _respawnDelayDurationTime = Time.time + _respawnDelayTime;
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdRespawnAll()
        {
            for (int i = 0; i < Players.Count; i++)
            {
                Players[i].CmdRespawn();
                Players[i].RpcRespawn();
            }
        }

        private void Update()
        {
            if (IsInRespawnMode && Time.time > _respawnDelayDurationTime)
            {
                _respawnDelayDurationTime = RespawnDelayDurationTimeMin;
                CmdRespawnAll();
            }
        }

        private bool IsInRespawnMode => _respawnDelayDurationTime != RespawnDelayDurationTimeMin;
    }
}
