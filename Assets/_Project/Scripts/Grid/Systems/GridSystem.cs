using System.Collections.Generic;
using Entities.Grid;
using UnityEngine;

namespace Systems.Grid
{
    public class GridSystem : IGridSystem
    {
        private Dictionary<Vector2Int, GridCell> _grid = new Dictionary<Vector2Int, GridCell>();

        public void InitializeGrid(Transform levelRoot)
        {
            _grid.Clear();

            GridTile[] allTiles = levelRoot.GetComponentsInChildren<GridTile>();

            foreach (var tile in allTiles)
            {
                foreach (var pos in tile.GetOccupiedPositions())
                {
                    if (!_grid.ContainsKey(pos))
                    {
                        _grid.Add(pos, new GridCell(pos, tile));
                    }
                    else
                    {
                        Debug.LogError($"Конфликт сетки на координате {pos}!");
                    }
                }
            }
        }

        public bool IsCoordinateValid(Vector2Int coordinate)
        {
            return _grid.ContainsKey(coordinate);
        }

        public bool TryGetCell(Vector2Int coordinate, out GridCell cell)
        {
            return _grid.TryGetValue(coordinate, out cell);
        }

        public bool CanMove(Vector2Int from, Vector2Int to)
        {
            if (!TryGetCell(from, out GridCell fromCell) || !TryGetCell(to, out GridCell toCell))
                return false;

            if (!toCell.Tile.IsPassable)
                return false;

            Vector2Int diff = to - from;
            if (Mathf.Abs(diff.x) + Mathf.Abs(diff.y) != 1)
                return false;

            return true;
        }

        public void MoveEntity(Vector2Int from, Vector2Int to)
        {
            if (CanMove(from, to) && TryGetCell(from, out GridCell fromCell) && TryGetCell(to, out GridCell toCell))
            {
                var entity = fromCell.Occupant.Value;
                fromCell.Occupant.Value = null;
                toCell.Occupant.Value = entity;
                entity.Position.Value = to; 
            }
        }

        public bool TryPlaceEntity(Entity entity, Vector2Int coordinate)
        {
            if (TryGetCell(coordinate, out GridCell cell) && cell.IsEmpty)
            {
                cell.Occupant.Value = entity;
                entity.Position.Value = coordinate;
                return true;
            }
            return false;
        }

        public void RemoveEntity(Vector2Int coordinate)
        {
            if (TryGetCell(coordinate, out GridCell cell))
            {
                cell.Occupant.Value = null;
            }
        }
    }
}