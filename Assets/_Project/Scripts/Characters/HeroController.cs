using Entities.Grid;
using LitMotion;
using LitMotion.Extensions;
using R3;
using Systems.Combat;
using Systems.Grid;
using Systems.Input;
using UI.Entity;
using UnityEngine;

namespace Characters
{
    [RequireComponent(typeof(CharacterAnimator))]
    public class HeroController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _moveSpeed = 5f;

        [Header("UI")]
        [SerializeField] private EntityHealthView _healthView;

        private CharacterAnimator _animator;
        private HeroEntity _entity;
        private IGridSystem _gridSystem;
        private CombatManager _combatManager;
        private IInputService _inputService;
        private CompositeDisposable _disposables = new();

        private bool _isVisuallyMoving;

        private void Awake()
        {
            _animator = GetComponent<CharacterAnimator>();
        }

        public void Initialize(HeroEntity entity, 
                               IGridSystem gridSystem, 
                               IInputService inputService,
                               CombatManager combatManager)
        {
            _entity = entity;
            _gridSystem = gridSystem;
            _inputService = inputService;
            _combatManager = combatManager;

            if (_healthView != null)
            {
                _healthView.Initialize(_entity);
            }

            transform.position = new Vector3(
                _entity.Position.Value.x + 0.5f,
                0.5f,
                _entity.Position.Value.y + 0.5f
            );

            transform.rotation = Quaternion.Euler(25, 0, 0);

            _entity.Position
                .Skip(1)
                .Subscribe(MoveVisually)
                .AddTo(_disposables);

            Observable.CombineLatest(
                _entity.State,
                _entity.Direction,
                (state, dir) => (state, dir)
            ).Subscribe(data =>
            {
                _animator.Play(data.state, data.dir);
            }).AddTo(_disposables);

            _inputService.OnMoveInput
                .Subscribe(OnMoveInputReceived)
                .AddTo(_disposables);

            _entity.Destroyed
                .Subscribe(_ => Destroy(gameObject))
                .AddTo(_disposables);
        }

        private void OnMoveInputReceived(Vector2Int inputDir)
        {
            // Блокируем ввод, если герой уже двигается, атакует или мертв
            if (_entity == null || _isVisuallyMoving ||
                _entity.State.Value == CharacterState.Death ||
                _entity.State.Value == CharacterState.Attack)
                return;

            // Разворачиваем героя в сторону движения
            _entity.Direction.Value = inputDir;

            Vector2Int targetPos = _entity.Position.Value + inputDir;

            if (_gridSystem.CanMove(_entity.Position.Value, targetPos))
            {
                // Проверяем, есть ли кто-то в клетке
                if (_gridSystem.TryGetCell(targetPos, out var cell))
                {
                    if (cell.IsEmpty)
                    {
                        // Клетка пуста - идем
                        _gridSystem.MoveEntity(_entity.Position.Value, targetPos);
                    }
                    else
                    {
                        if (cell.Occupant.Value is CharacterEntity targetCharacter)
                        {
                            _entity.State.Value = CharacterState.Attack; // Триггерим анимацию
                            _combatManager.ResolveMeleeAttack(_entity, targetCharacter);

                            Observable.Timer(System.TimeSpan.FromSeconds(0.5f))
                             .Subscribe(_ =>
                             {
                                 if (_entity.State.Value != CharacterState.Death)
                                 {
                                     _entity.State.Value = CharacterState.Idle;
                                 }
                             })
                             .AddTo(_disposables);
                        }
                    }
                }
            }
        }

        private void MoveVisually(Vector2Int targetGridPos)
        {
            _isVisuallyMoving = true;
            _entity.State.Value = CharacterState.Walk;

            Vector3 targetWorldPos = new Vector3(
                targetGridPos.x + 0.5f,
                0.5f,
                targetGridPos.y + 0.5f
            );

            float duration = 1f / _moveSpeed;

            LMotion.Create(transform.position, targetWorldPos, duration)
                .WithEase(Ease.Linear)
                .WithOnComplete(() =>
                {
                    if (_entity.State.Value == CharacterState.Walk)
                    {
                        _entity.State.Value = CharacterState.Idle;
                    }
                    _isVisuallyMoving = false;
                })
                .BindToPosition(transform);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}

