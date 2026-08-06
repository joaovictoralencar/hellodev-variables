using UnityEngine;
using UnityEngine.UI;
using HelloDev.Variables;

namespace HelloDev.Variables.Samples.BasicHealthExample
{
    /// <summary>
    /// Debug UI controller for the health example.
    /// Provides buttons for damage, heal, and reset actions.
    /// </summary>
    public class HealthDebugUI : MonoBehaviour
    {
        [SerializeField] private HealthManager _healthManager;
        [SerializeField] private Button _damageButton;
        [SerializeField] private Button _healButton;
        [SerializeField] private Button _resetButton;

        private void Start()
        {
            if (_damageButton != null)
                _damageButton.onClick.AddListener(_healthManager.TakeDamage);

            if (_healButton != null)
                _healButton.onClick.AddListener(_healthManager.Heal);

            if (_resetButton != null)
                _resetButton.onClick.AddListener(_healthManager.ResetHealth);
        }

        private void OnDestroy()
        {
            if (_damageButton != null)
                _damageButton.onClick.RemoveListener(_healthManager.TakeDamage);

            if (_healButton != null)
                _healButton.onClick.RemoveListener(_healthManager.Heal);

            if (_resetButton != null)
                _resetButton.onClick.RemoveListener(_healthManager.ResetHealth);
        }
    }
}