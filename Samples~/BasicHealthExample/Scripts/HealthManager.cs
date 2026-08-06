using UnityEngine;

namespace HelloDev.Variables.Samples.BasicHealthExample
{
    /// <summary>
    /// Simple health manager demonstrating SO variable usage.
    /// Manages player health using a FloatVariable_SO and provides debug controls.
    /// </summary>
    public class HealthManager : MonoBehaviour
    {
        [SerializeField] private FloatVariable_SO _health;
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _damageAmount = 10f;
        [SerializeField] private float _healAmount = 25f;

        private void Start()
        {
            // Ensure health is initialized
            if (_health != null)
            {
                _health.SetValue(_maxHealth);
                _health.OnValueChanged.AddListener(OnHealthChanged);
            }
        }

        public void TakeDamage()
        {
            if (_health != null)
            {
                _health.Value -= _damageAmount;
                _health.Value = Mathf.Max(_health.Value, 0f);
                
                if (_health.Value <= 0f)
                {
                    OnDeath();
                }
            }
        }

        public void Heal()
        {
            if (_health != null)
            {
                _health.Value += _healAmount;
                _health.Value = Mathf.Min(_health.Value, _maxHealth);
            }
        }

        public void ResetHealth()
        {
            if (_health != null)
            {
                _health.ResetToDefault();
                Debug.Log("Health reset to default");
            }
        }

        private void OnHealthChanged(float newHealth)
        {
            Debug.Log($"Health changed: {newHealth}/{_maxHealth}");
        }

        private void OnDeath()
        {
            Debug.Log("Player died!");
        }
    }
}