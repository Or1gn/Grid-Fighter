using Core.Grid;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace System.Grid
{
    public class LevelGenerator
    {
        private readonly RoomModuleSO[] _availableModules;
        private readonly RoomModuleSO _endCapModule; // Ссылка на corridor-end.fbx

        public LevelGenerator(RoomModuleSO[] modules, RoomModuleSO endCap)
        {
            _availableModules = modules;
            _endCapModule = endCap;
        }

        public async UniTask<LevelData> GenerateLevelAsync(int criticalPathLength)
        {
            var levelData = new LevelData();
            Vector2Int currentPos = Vector2Int.zero;
            levelData.StartPosition = currentPos;

            SocketDirection incomingSocket = SocketDirection.None;

            for (int i = 0; i < criticalPathLength; i++)
            {
                List<SocketDirection> validDirections = GetValidDirections(currentPos, levelData);
                if (validDirections.Count == 0) break; // Зашли в глухой угол

                // Берем случайное направление из доступных
                SocketDirection outgoingSocket = validDirections[UnityEngine.Random.Range(0, validDirections.Count)];

                SocketDirection requiredSockets = incomingSocket | outgoingSocket;
                if (i == criticalPathLength - 1) requiredSockets = incomingSocket;

                // Ищем модуль, который не просто подходит по сокетам, но и ВЛЕЗАЕТ в сетку
                var (module, rotationSteps) = FindFittingModule(currentPos, requiredSockets, levelData, isCritical: true);

                if (module == null)
                {
                    Debug.LogWarning($"Не удалось найти модуль, который поместится в {currentPos}. Попробуй увеличить пул комнат.");
                    break; // Откат назад здесь не реализован для простоты, просто прерываем путь
                }

                var cellData = new CellData
                {
                    Position = currentPos,
                    Module = module,
                    Rotation = Quaternion.Euler(0, rotationSteps * 90, 0),
                    IsCriticalPath = true
                };

                // "Резервируем" все клетки, которые занимает этот модуль
                var footprint = module.GetRotatedFootprint(rotationSteps);
                foreach (var offset in footprint)
                {
                    levelData.Grid[currentPos + offset] = cellData;
                }

                currentPos += DirectionToVector(outgoingSocket);
                incomingSocket = GetOpposite(outgoingSocket);

                if (i % 5 == 0) await UniTask.Yield();
            }

            levelData.BossPosition = currentPos;
            await CloseDeadEndsAsync(levelData);
            return levelData;
        }

        // Новый метод с проверкой коллизий Footprint'а
        private (RoomModuleSO module, int rotation) FindFittingModule(Vector2Int position, SocketDirection required, LevelData data, bool isCritical = false, bool forceEndCap = false)
        {
            var pool = forceEndCap ? new[] { _endCapModule } : _availableModules.Where(m => !m.IsEndCap).ToArray();
            pool = pool.OrderBy(x => UnityEngine.Random.value).ToArray();

            foreach (var mod in pool)
            {
                if (isCritical && mod.IsEndCap) continue;

                for (int r = 0; r < 4; r++)
                {
                    if (mod.Fits(required, r))
                    {
                        // Проверяем Footprint
                        bool fitsGrid = true;
                        var footprint = mod.GetRotatedFootprint(r);
                        foreach (var offset in footprint)
                        {
                            Vector2Int targetCell = position + offset;
                            // Если клетка не корень и уже занята — этот модуль сюда не лезет
                            if (offset != Vector2Int.zero && data.Grid.ContainsKey(targetCell))
                            {
                                fitsGrid = false;
                                break;
                            }
                        }

                        if (fitsGrid) return (mod, r);
                    }
                }
            }
            return (null, 0); // Ничего не подошло
        }

        // Жестко привязываем векторы к осям (Z+ это North)
        private Vector2Int DirectionToVector(SocketDirection dir) => dir switch
        {
            SocketDirection.North => new Vector2Int(0, 1),
            SocketDirection.East => new Vector2Int(1, 0),
            SocketDirection.South => new Vector2Int(0, -1),
            SocketDirection.West => new Vector2Int(-1, 0),
            _ => Vector2Int.zero
        };
        
        private async UniTask CloseDeadEndsAsync(LevelData level)
        {
            var newCells = new Dictionary<Vector2Int, CellData>();

            foreach (var kvp in level.Grid)
            {
                var cell = kvp.Value;
                var currentSockets = RoomModuleSO.RotateSockets(cell.Module.OpenSockets, (int)cell.Rotation.eulerAngles.y / 90);

                foreach (SocketDirection dir in System.Enum.GetValues(typeof(SocketDirection)))
                {
                    if (dir == SocketDirection.None) continue;

                    if (currentSockets.HasFlag(dir))
                    {
                        Vector2Int neighborPos = cell.Position + DirectionToVector(dir);
                        if (!level.Grid.ContainsKey(neighborPos) && !newCells.ContainsKey(neighborPos))
                        {
                            // Размещаем End-модуль, повернутый к выходу
                            SocketDirection requiredEntry = GetOpposite(dir);
                            var (endModule, rotSteps) = FindMatchingModule(requiredEntry, false, true);

                            newCells[neighborPos] = new CellData
                            {
                                Position = neighborPos,
                                Module = endModule ?? _endCapModule,
                                Rotation = Quaternion.Euler(0, rotSteps * 90, 0)
                            };
                        }
                    }
                }
            }

            foreach (var nc in newCells) level.Grid[nc.Key] = nc.Value;
            await UniTask.Yield();
        }

        private (RoomModuleSO module, int rotation) FindMatchingModule(SocketDirection required, bool isCritical = false, bool forceEndCap = false)
        {
            var pool = forceEndCap ? new[] { _endCapModule } : _availableModules.Where(m => !m.IsEndCap).ToArray();
            // Перемешиваем для рандомизации
            pool = pool.OrderBy(x => UnityEngine.Random.value).ToArray();

            foreach (var mod in pool)
            {
                for (int r = 0; r < 4; r++)
                {
                    if (mod.Fits(required, r))
                    {
                        // Для критического пути избегаем глухих тупиков
                        if (isCritical && mod.IsEndCap) continue;
                        return (mod, r);
                    }
                }
            }
            return (pool[0], 0); // Fallback
        }

        private SocketDirection GetOpposite(SocketDirection dir) => dir switch
        {
            SocketDirection.North => SocketDirection.South,
            SocketDirection.South => SocketDirection.North,
            SocketDirection.East => SocketDirection.West,
            SocketDirection.West => SocketDirection.East,
            _ => SocketDirection.None
        };

        private List<SocketDirection> GetValidDirections(Vector2Int pos, LevelData data)
        {
            var valid = new List<SocketDirection>();
            if (!data.Grid.ContainsKey(pos + Vector2Int.up)) valid.Add(SocketDirection.North);
            if (!data.Grid.ContainsKey(pos + Vector2Int.down)) valid.Add(SocketDirection.South);
            if (!data.Grid.ContainsKey(pos + Vector2Int.right)) valid.Add(SocketDirection.East);
            if (!data.Grid.ContainsKey(pos + Vector2Int.left)) valid.Add(SocketDirection.West);
            return valid;
        }
    }
}

