using System.Collections.Generic;
using UnityEngine;

namespace Core.Grid
{
    [CreateAssetMenu(fileName = "RoomModuleSO", menuName = "GridFighter/RoomModuleSO")]
    public class RoomModuleSO : ScriptableObject
    {
        [Header("Visuals")]
        public GameObject Prefab; 

        [Header("Sockets (Default rotation - facing North)")]
        public SocketDirection OpenSockets;

        [Header("Settings")]
        public bool IsEndCap;

        [Header("Footprint (Local Grid Coordinates)")]
        [Tooltip("Какие клетки занимает модуль. (0,0) - это всегда корень. Для комнаты 2x2 добавь (1,0), (0,1), (1,1)")]
        public Vector2Int[] LocalFootprint = new Vector2Int[] { Vector2Int.zero };

        public bool Fits(SocketDirection requiredConnections, int rotationSteps)
        {
            SocketDirection rotatedSockets = RotateSockets(OpenSockets, rotationSteps);
            return (rotatedSockets & requiredConnections) == requiredConnections;
        }

        public static SocketDirection RotateSockets(SocketDirection original, int steps)
        {
            int result = (int)original;
            for (int i = 0; i < steps; i++)
            {
                // Сдвиг по часовой стрелке: North(1)->East(2)->South(4)->West(8)
                result = ((result << 1) | (result >> 3)) & 15;
            }
            return (SocketDirection)result;
        }

        // Вращение физического следа префаба
        public List<Vector2Int> GetRotatedFootprint(int rotationSteps)
        {
            var rotated = new List<Vector2Int>();
            foreach (var pos in LocalFootprint)
            {
                int x = pos.x;
                int y = pos.y;

                // Математика поворота Vector2Int на 90 градусов по часовой
                for (int i = 0; i < rotationSteps; i++)
                {
                    int temp = x;
                    x = y;
                    y = -temp;
                }
                rotated.Add(new Vector2Int(x, y));
            }
            return rotated;
        }
    }
}
