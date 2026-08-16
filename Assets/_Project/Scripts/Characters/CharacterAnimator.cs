using Entities.Grid;
using UnityEngine;

namespace Characters
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] private CharacterAnimationSO _characterAnimationSO;
        private SpriteRenderer _spriteRenderer;

        private AnimationSequence _currentSequence;
        private float _timer;
        private int _currentFrame;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Play(CharacterState state, Vector2Int direction)
        {
            var newSequence = _characterAnimationSO.GetSequence(state, direction);
            if (newSequence == null || newSequence == _currentSequence) return;

            _currentSequence = newSequence;
            _currentFrame = 0;
            _timer = 0f;
            _spriteRenderer.sprite = _currentSequence.Frames[_currentFrame];
        }

        private void Update()
        {
            if (_currentSequence == null || _currentSequence.Frames.Length <= 1) return;

            _timer += Time.deltaTime;
            float frameDuration = 1f / _currentSequence.FramesPerSecond;

            if (_timer >= frameDuration)
            {
                _timer -= frameDuration;
                _currentFrame++;

                if (_currentFrame >= _currentSequence.Frames.Length)
                {
                    _currentFrame = _currentSequence.Loop ? 0 : _currentSequence.Frames.Length - 1;
                }

                _spriteRenderer.sprite = _currentSequence.Frames[_currentFrame];
            }
        }
    }
}
