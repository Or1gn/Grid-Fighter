using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Grid
{
    [Flags]
    public enum SocketDirection
    {
        None = 0,
        North = 1 << 0, // Вверх (Z+)
        East = 1 << 1, // Вправо (X+)
        South = 1 << 2, // Вниз (Z-)
        West = 1 << 3  // Влево (X-)
    }

    public class CellData
    {
        public Vector2Int Position { get; set; }
        public RoomModuleSO Module { get; set; }
        public Quaternion Rotation { get; set; }
        public bool IsCriticalPath { get; set; }
    }

    public class LevelData
    {
        public Dictionary<Vector2Int, CellData> Grid { get; set; } = new Dictionary<Vector2Int, CellData>();
        public Vector2Int StartPosition { get; set; }
        public Vector2Int BossPosition { get; set; }
    }
}
