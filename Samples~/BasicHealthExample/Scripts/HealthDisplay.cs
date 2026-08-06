using UnityEngine;
using UnityEngine.UI;
using HelloDev.Variables;
using TMPro;

namespace HelloDev.Variables.Samples.BasicHealthExample
{
    /// <summary>
    /// Displays health value from a FloatVariable_SO using UI Text.
    /// Updates automatically when the variable changes.
    /// </summary>
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private FloatVariable_SO _health;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private Image _healthBar;
        [SerializeField] private float _maxHealth = 100f;

        private void Start()
        {
            if (_health != null)
            {
                _health.OnValueChanged.AddListener(UpdateDisplay);
                UpdateDisplay(_health.Value);
            }
        }

        private void UpdateDisplay(float currentHealth)
        {
            if (_healthText != null)
            {
                _healthText.text = $"Health: {currentHealth:F0}/{_maxHealth}";
            }

            if (_healthBar != null)
            {
                _healthBar.fillAmount = currentHealth / _maxHealth;
            }
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.OnValueChanged.RemoveListener(UpdateDisplay);
            }
        }
    }
}