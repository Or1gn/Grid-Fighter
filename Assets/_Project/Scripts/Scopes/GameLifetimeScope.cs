using Characters;
using Core.Grid;
using Entities.Grid;
using Events.Combat;
using Events.Grid;
using MessagePipe;
using Settings;
using System.Grid;
using Systems.Combat;
using Systems.Grid;
using Systems.Input;
using UI.Hud;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scopes
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private Settings.LevelData _startLevel;
        [SerializeField] private HeroController _heroPrefab;
        [SerializeField] private CameraManager _cameraManager;
        [SerializeField] private GridSystem1 _gridSystemPrefab;
        [SerializeField] private RoomModuleSO[] _allRoomModules;
        [SerializeField] private RoomModuleSO _endCapModule;

        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<LevelGeneratedEvent>(options);
            builder.RegisterMessageBroker<HeroDiedEvent>(options);
            builder.RegisterMessageBroker<HeroSpawnedEvent>(options);
            builder.RegisterMessageBroker<DamageDealtEvent>(options);

            RegisterSystems(builder);
            RegisterViewComponents(builder);
            RegisterEntryPoints(builder);

            builder.Register<LevelGenerator>(Lifetime.Singleton)
               .WithParameter(_allRoomModules)
               .WithParameter(_endCapModule);

            // Регистрируем GridSystem, создавая её на сцене
            builder.RegisterComponentInNewPrefab(_gridSystemPrefab, Lifetime.Scoped);

            // Регистрируем стартовый контроллер (EntryPoint)
            builder.RegisterEntryPoint<GameFlowController>();
        }

        void RegisterEntryPoints(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameInitializer>();
        }

        void RegisterSystems(IContainerBuilder builder)
        {
            builder.Register<GridSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<DesktopInputService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<CombatManager>(Lifetime.Singleton);
        }

        void RegisterViewComponents(IContainerBuilder builder)
        {
            if (_startLevel != null)
            {
                builder.RegisterInstance(_startLevel);
            }

            if (_heroPrefab != null)
            {
                builder.RegisterInstance(_heroPrefab);
            }

            if (_cameraManager != null)
            {
                builder.RegisterInstance(_cameraManager);
            }
        }
    }

    public class GameInitializer : IStartable
    {
        private readonly Settings.LevelData _startLevel;
        private readonly IGridSystem _gridSystem;
        private readonly IPublisher<LevelGeneratedEvent> _levelGeneratedPublisher;
        private readonly IPublisher<HeroSpawnedEvent> _heroSpawnedEvent;
        private readonly HeroController _heroPrefab;
        private readonly IInputService _inputService;
        private readonly CameraManager _cameraManager;
        private readonly CombatManager _combatManager;

        public GameInitializer(
            Settings.LevelData startLevel,
            IGridSystem gridSystem,
            IPublisher<LevelGeneratedEvent> levelGeneratedPublisher,
            IPublisher<HeroSpawnedEvent> heroSpawnedEvent,
            HeroController heroPrefab,
            IInputService inputService,
            CameraManager cameraManager,
            CombatManager combatManager) 
        {
            _startLevel = startLevel;
            _gridSystem = gridSystem;
            _levelGeneratedPublisher = levelGeneratedPublisher;
            _heroSpawnedEvent = heroSpawnedEvent;
            _heroPrefab = heroPrefab;
            _inputService = inputService;
            _cameraManager = cameraManager;
            _combatManager = combatManager;
        }

        public void Start()
        {
            if (_startLevel.LevelPrefab == null)
            {
                Debug.LogError("В LevelData не назначен префаб уровня!");
                return;
            }

            GameObject levelInstance = Object.Instantiate(_startLevel.LevelPrefab);
            _gridSystem.InitializeGrid(levelInstance.transform);

            SpawnHero(_startLevel.PlayerSpawnPosition);

            SpawnEnemies();

            _levelGeneratedPublisher.Publish(new LevelGeneratedEvent());
        }

        private void SpawnHero(Vector2Int spawnCoordinate)
        {
            if (_heroPrefab == null)
            {
                Debug.LogError("Префаб героя не зарегистрирован в LifetimeScope!");
                return;
            }

            var heroEntity = new HeroEntity();

            if (_gridSystem.TryPlaceEntity(heroEntity, spawnCoordinate))
            {
                Vector3 worldPos = new Vector3(
                                            spawnCoordinate.x + 0.5f,
                                            0.5f,
                                            spawnCoordinate.y + 0.5f
                                        );

                HeroController heroInstance = Object.Instantiate(_heroPrefab, worldPos, Quaternion.identity);

                heroInstance.Initialize(heroEntity, _gridSystem, _inputService, _combatManager);

                _heroSpawnedEvent.Publish(new HeroSpawnedEvent(heroEntity));

                _cameraManager.SetTarget(heroInstance.transform);
            }
            else
            {
                Debug.LogError($"Не удалось заспавнить героя! Координата {spawnCoordinate} занята или не является проходной.");
            }
        }

        private void SpawnEnemies()
        {
            foreach (var enemyData in _startLevel.Enemies)
            {
                if (enemyData.Prefab == null)
                {
                    Debug.LogWarning($"У врага на позиции {enemyData.SpawnPosition} не указан префаб!");
                    continue;
                }

                var enemyEntity = new EnemyEntity();

                if (_gridSystem.TryPlaceEntity(enemyEntity, enemyData.SpawnPosition))
                {
                    Vector3 worldPos = new Vector3(
                        enemyData.SpawnPosition.x + 0.5f,
                        0.5f,
                        enemyData.SpawnPosition.y + 0.5f
                    );

                    GameObject enemyInstance = Object.Instantiate(enemyData.Prefab, worldPos, Quaternion.identity);

                    if (enemyInstance.TryGetComponent<EnemyController>(out var enemyController))
                    {
                        enemyController.Initialize(enemyEntity);
                    }
                    else
                    {
                        Debug.LogWarning($"На префабе {enemyData.Prefab.name} не висит скрипт EnemyController!");
                    }
                }
                else
                {
                    Debug.LogError($"Не удалось заспавнить врага! Координата {enemyData.SpawnPosition} занята или не является проходной.");
                }
            }
        }
    }
}


