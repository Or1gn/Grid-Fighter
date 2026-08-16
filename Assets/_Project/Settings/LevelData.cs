using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
    public class LevelData : ScriptableObject
    {
        public GameObject LevelPrefab;
        public Vector2Int PlayerSpawnPosition;

        public List<EnemySpawnData> Enemies = new List<EnemySpawnData>();
    }

    [System.Serializable]
    public struct EnemySpawnData
    {
        public Vector2Int SpawnPosition;
        public int Level;
        public GameObject Prefab;
    }
}
