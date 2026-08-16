using Entities.Grid;
using Events.Combat;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI.Hud
{
    public class HealthController : MonoBehaviour
    {
        [SerializeField] private Image[] _hearts;

        [SerializeField] private Color _activeColor = Color.white;
        [SerializeField] private Color _inactiveColor = new Color(1f, 1f, 1f, 0.3f);

        private ISubscriber<HeroSpawnedEvent> _heroSpawnedEvent;

        private CompositeDisposable _disposables = new();

        [Inject]
        public void Construct(ISubscriber<HeroSpawnedEvent> heroSpawnedEvent) 
        {
            _heroSpawnedEvent = heroSpawnedEvent;
        }

        private void Start()
        {
            _heroSpawnedEvent
                .Subscribe(heroSpawnedEvent => Initialize(heroSpawnedEvent.Hero))
                .AddTo(_disposables);
        }

        public void Initialize(HeroEntity heroEntity)
        {
            heroEntity.CurrentHealth
                .Subscribe(UpdateHeartsVisual)
                .AddTo(_disposables);
        }

        private void UpdateHeartsVisual(int currentLives)
        {
            for (int i = 0; i < _hearts.Length; i++)
            {
                _hearts[i].color = i < currentLives ? _activeColor : _inactiveColor;
            }
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}

