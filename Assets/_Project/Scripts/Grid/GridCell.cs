using Entities.Grid;
using UnityEngine;
using R3; 

namespace Systems.Grid
{
    public class GridCell
    {
        public Vector2Int Coordinate { get; }
        public GridTile Tile { get; } 

        public ReactiveProperty<Entity> Occupant { get; } = new ReactiveProperty<Entity>();

        public bool IsEmpty => Occupant.Value == null;

        public GridCell(Vector2Int coordinate, GridTile tile)
        {
            Coordinate = coordinate;
            Tile = tile;
        }
    }
}