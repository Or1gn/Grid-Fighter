using R3;
using UnityEngine;

namespace Entities.Grid
{
    public enum CharacterState
    {
        Idle,
        Walk,
        Attack,
        Death
    }

    public abstract class Entity
    {
        public ReactiveProperty<Vector2Int> Position { get; } = new ReactiveProperty<Vector2Int>();
        public ReactiveCommand Destroyed { get; } = new ReactiveCommand();
    }

    public abstract class CharacterEntity : Entity
    {
        public ReactiveProperty<int> MaxHealth { get; } = new ReactiveProperty<int> { Value = 100 };
        public ReactiveProperty<int> CurrentHealth { get; set; } = new ReactiveProperty<int> { Value = 100 };
        public ReactiveProperty<int> Attack { get; } = new ReactiveProperty<int> { Value = 60 };
        public ReactiveProperty<Vector2Int> Direction { get; } = new ReactiveProperty<Vector2Int>(Vector2Int.down);
        public ReactiveProperty<CharacterState> State { get; } = new ReactiveProperty<CharacterState>(CharacterState.Idle);
    }

    public class HeroEntity : CharacterEntity
    {

    }

    public class EnemyEntity : CharacterEntity
    {
    }

    public class ItemEntity : Entity
    {
        public int ExperienceReward { get; set; }
    }
}
