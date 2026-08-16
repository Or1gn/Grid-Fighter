using Core.Grid;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;

namespace System.Grid
{
    public class GridSystem1 : MonoBehaviour
    {
        public readonly ReactiveProperty<bool> IsGridReady = new ReactiveProperty<bool>(false);

        private LevelData _currentLevelData;
        private const float CELL_SIZE = 5f; // Размер тайла сетки в Unity-юнитах

        [Inject]
        public void Construct()
        {
            // Инъекция необходимых зависимостей через VContainer
        }

        public async UniTask InitializeGridAsync(LevelData data)
        {
            IsGridReady.Value = false;
            _currentLevelData = data;

            int spawnedCount = 0;
            foreach (var cell in _currentLevelData.Grid.Values)
            {
                Vector3 worldPos = new Vector3(cell.Position.x * CELL_SIZE, 0, cell.Position.y * CELL_SIZE);

                // Инстанцирование префаба. В продакшене можно подключить ObjectPool
                Instantiate(cell.Module.Prefab, worldPos, cell.Rotation, transform);

                spawnedCount++;
                if (spawnedCount % 5 == 0)
                {
                    await UniTask.Yield(); // Распределяем нагрузку по кадрам
                }
            }

            // По завершении сетка готова к работе CombatManager и Entity
            IsGridReady.Value = true;
        }

        public bool IsWalkable(Vector2Int gridPos)
        {
            return _currentLevelData.Grid.ContainsKey(gridPos);
        }
    }

}
