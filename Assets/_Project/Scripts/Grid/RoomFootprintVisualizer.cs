using UnityEngine;

namespace System
{
    public class RoomFootprintVisualizer : MonoBehaviour
    {
        [Header("Размер одной клетки")]
        public float CellSize = 5f; // Твоя константа из GridSystem

        [Header("Клетки, которые занимает меш")]
        [Tooltip("0,0 — это клетка пивота. (1,0) — смещение на одну клетку вправо по оси X.")]
        public Vector2Int[] LocalFootprint = new Vector2Int[] { Vector2Int.zero };

        private void OnDrawGizmos()
        {
            // Привязываем отрисовку строго к локальным координатам префаба,
            // чтобы квадраты вращались вместе с объектом
            Gizmos.matrix = transform.localToWorldMatrix;

            foreach (var pos in LocalFootprint)
            {
                // Переводим логические координаты (X, Y) в физические (X, Z)
                Vector3 center = new Vector3(pos.x * CellSize, 0, pos.y * CellSize);
                Vector3 size = new Vector3(CellSize, 0.1f, CellSize);

                // Рисуем полупрозрачную зеленую заливку
                Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
                Gizmos.DrawCube(center, size);

                // Рисуем яркие зеленые контуры
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(center, size);
            }

            // Отдельно выделим саму клетку пивота (0,0) красным цветом,
            // чтобы ты всегда видел, где находится "якорь" комнаты
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(CellSize * 0.9f, 0.2f, CellSize * 0.9f));
        }
    }
}

