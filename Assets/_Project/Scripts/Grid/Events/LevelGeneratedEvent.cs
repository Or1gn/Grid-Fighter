using Entities.Grid;
using Systems.Grid;

namespace Events.Grid
{
    public readonly struct LevelGeneratedEvent
    {
        public readonly IGridSystem Grid;
        public LevelGeneratedEvent(IGridSystem grid) => Grid = grid;
    }

    public readonly struct EntitySpawnedEvent
    {
        public readonly Entity Model;
        public EntitySpawnedEvent(Entity model) => Model = model;
    }
}
