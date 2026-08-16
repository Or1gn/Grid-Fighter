using Cysharp.Threading.Tasks;
using System.Grid;
using Systems.Grid;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Grid
{
    public class GameFlowController : IAsyncStartable
    {
        private readonly LevelGenerator _levelGenerator;
        private readonly GridSystem1 _gridSystem;

        [Inject]
        public GameFlowController(LevelGenerator levelGenerator, GridSystem1 gridSystem)
        {
            _levelGenerator = levelGenerator;
            _gridSystem = gridSystem;
        }

        public async UniTask StartAsync(System.Threading.CancellationToken cancellation)
        {
            Debug.Log("Начинаем абстрактную генерацию уровня...");

            // Генерируем 10 комнат критического пути (длина данжа)
            // Алгоритм работает в фоне и не вешает UI
            LevelData levelData = await _levelGenerator.GenerateLevelAsync(10);

            Debug.Log("Абстрактная модель готова. Спавн 3D объектов на сцене...");

            // Передаем данные в систему сетки для расстановки префабов
            await _gridSystem.InitializeGridAsync(levelData);

            Debug.Log("Уровень полностью загружен! Можно спавнить героя.");
        }
    }
}
