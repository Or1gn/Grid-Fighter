using Entities.Grid;
using UnityEngine;

namespace Systems.Grid
{
    public interface IGridSystem
    {
        void InitializeGrid(Transform levelRoot);

        bool IsCoordinateValid(Vector2Int coordinate);
        bool TryGetCell(Vector2Int coordinate, out GridCell cell);

        bool CanMove(Vector2Int from, Vector2Int to);
        void MoveEntity(Vector2Int from, Vector2Int to);

        bool TryPlaceEntity(Entity entity, Vector2Int coordinate);
        void RemoveEntity(Vector2Int coordinate);
    }
}