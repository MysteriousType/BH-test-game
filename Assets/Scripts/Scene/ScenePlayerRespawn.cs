namespace Assets.Scripts.Scene
{
    using Assets.Scripts.Player;
    using Mirror;

    public class ScenePlayerRespawn : NetworkBehaviour
    {
        private readonly SyncList<Player> Players = new SyncList<Player>();

        [Command(requiresAuthority = false)]
        public void AddPlayer(Player player) => Players.Add(player);

        [Command(requiresAuthority = false)]
        public void RemovePlayer(Player player) => Players.Remove(player);

        [Command(requiresAuthority = false)]
        public void RespawnAll()
        {
            for (int i = 0; i < Players.Count; i++)
            {
                Players[i].CmdRespawn();
                Players[i].RpcRespawn();
            }
        }
    }
}
