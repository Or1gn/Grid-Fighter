using R3;
using UnityEngine;

namespace Systems.Input
{
    public interface IInputService
    {
        Observable<Vector2Int> OnMoveInput { get; }
    }
}