using R3;
using System;
using UnityEngine;
using UnityEngine.InputSystem; // Подключаем новую систему ввода
using VContainer.Unity;

namespace Systems.Input
{
    public class DesktopInputService : IInputService, ITickable, IDisposable
    {
        private readonly Subject<Vector2Int> _onMoveInput = new Subject<Vector2Int>();

        public Observable<Vector2Int> OnMoveInput => _onMoveInput;

        public void Tick()
        {
            // Получаем текущую клавиатуру
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            // Используем wasPressedThisFrame вместо старого GetKeyDown
            if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame)
                _onMoveInput.OnNext(Vector2Int.up);
            else if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame)
                _onMoveInput.OnNext(Vector2Int.down);
            else if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
                _onMoveInput.OnNext(Vector2Int.right);
            else if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
                _onMoveInput.OnNext(Vector2Int.left);
        }

        public void Dispose()
        {
            _onMoveInput.Dispose();
        }
    }
}