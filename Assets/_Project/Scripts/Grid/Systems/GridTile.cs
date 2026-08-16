using System.Collections.Generic;
using UnityEngine;

namespace Systems.Grid
{
    public class GridTile : MonoBehaviour
    {
        public bool IsPassable = true;

        public List<Vector2Int> LocalCells = new List<Vector2Int> { Vector2Int.zero };

        public List<Vector2Int> GetOccupiedPositions()
        {
            List<Vector2Int> worldPositions = new List<Vector2Int>();

            foreach (var localCell in LocalCells)
            {
                Vector3 localOffset = new Vector3(localCell.x, 0, localCell.y);

                Vector3 rotatedOffset = transform.rotation * localOffset;

                int worldX = Mathf.RoundToInt(transform.position.x + rotatedOffset.x);
                int worldY = Mathf.RoundToInt(transform.position.z + rotatedOffset.z);

                worldPositions.Add(new Vector2Int(worldX, worldY));
            }

            return worldPositions;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = IsPassable ? new Color(0, 1, 0, 0.4f) : new Color(1, 0, 0, 0.4f);

            var positions = GetOccupiedPositions();

            foreach (var pos in positions)
            {
                Vector3 worldPos = new Vector3(pos.x, 0.1f, pos.y); 
                Gizmos.DrawCube(worldPos, new Vector3(0.9f, 0.1f, 0.9f));
            }
        }
    }
}