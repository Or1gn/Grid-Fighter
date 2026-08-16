using Entities.Grid;
using R3;
using UI.Entity;
using UnityEngine;

namespace Characters
{
    [RequireComponent(typeof(CharacterAnimator))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EntityHealthView _healthView;

        private CharacterAnimator _animator;
        private EnemyEntity _entity;
        private CompositeDisposable _disposables = new();

        private void Awake()
        {
            _animator = GetComponent<CharacterAnimator>();
        }

        public void Initialize(EnemyEntity entity)
        {
            _entity = entity;

            if (_healthView != null)
            {
                _healthView.Initialize(_entity);
            }
            else
            {
                Debug.LogWarning("EntityLevelView не назначен в EnemyController!");
            }

            Observable.CombineLatest(
                _entity.State,
                _entity.Direction,
                (state, dir) => (state, dir)
            ).Subscribe(data =>
            {
                if (_animator != null)
                {
                    _animator.Play(data.state, data.dir);
                }
            }).AddTo(_disposables);

            _entity.Destroyed
                .Subscribe(_ => OnDeath())
                .AddTo(_disposables);

            transform.rotation = Quaternion.Euler(25, 0, 0);
        }

        private void OnDeath()
        {
            Observable.Timer(System.TimeSpan.FromSeconds(1f))
            .Subscribe(_ =>
            {
                if (gameObject != null)
                {
                    Destroy(gameObject);
                }
            })
            .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
