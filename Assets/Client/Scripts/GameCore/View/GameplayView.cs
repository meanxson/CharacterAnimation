using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Client
{
    public class GameplayView : MonoBehaviour
    {
        [SerializeField] private Slider _manaSlider;

        private PlayerBehaviour _player;
        private const float effectDuration = 0.5f;

        [Inject]
        public void Constructor(PlayerBehaviour player)
        {
            _player = player;
        }

        private void OnEnable()
        {
            _player.ManaChanged += OnManaChange;
        }

        private void OnDisable()
        {
            _player.ManaChanged -= OnManaChange;
        }

        private void Start()
        {
            _manaSlider.value = _player.ManaPoint;
        }

        private void OnManaChange(float value)
        {
            _manaSlider.DOValue(value, effectDuration);
        }
    }
}