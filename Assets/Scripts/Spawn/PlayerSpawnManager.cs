namespace Assets.Scripts.Spawn
{
    using UnityEngine;

    public class PlayerSpawnManager : MonoBehaviour
    {
        public static PlayerSpawnManager Instance;

        [SerializeField]
        private Vector3[] _spawnPointPositions;

        public Vector3 GetRandomSpawnPosition()
        {
            int index = Random.Range(0, _spawnPointPositions.Length);
            return _spawnPointPositions[index];
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance == this)
            {
                Destroy(gameObject);
            }
        }
    }
}
