using Entities.Grid;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Entity
{
    public class EntityHealthView : MonoBehaviour
    {
        [SerializeField] private Image _healthFillImage; 
        [SerializeField] private TextMeshProUGUI _healthText; 

        private CompositeDisposable _disposables = new();

        public void Initialize(CharacterEntity entity)
        {
            Observable.CombineLatest(
                entity.CurrentHealth,
                entity.MaxHealth,
                (current, max) => (current, max)
            ).Subscribe(healthData =>
            {
                UpdateHealthUI(healthData.current, healthData.max);
            }).AddTo(_disposables);
        }

        private void UpdateHealthUI(int current, int max)
        {
            if (_healthFillImage != null && max > 0)
            {
                _healthFillImage.fillAmount = (float)current / max;
            }

            if (_healthText != null)
            {
                _healthText.text = $"{current}";
            }
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}

