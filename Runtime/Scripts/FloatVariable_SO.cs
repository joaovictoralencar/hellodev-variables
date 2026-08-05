using UnityEngine;
using UnityEngine.Events;
using HelloDev.Utils;

namespace HelloDev.Variables
{
    [CreateAssetMenu(menuName = "HelloDev/Variables/Float Variable", fileName = "FloatVariable_SO")]
    public class FloatVariable_SO : VariableBase_SO
    {
        [SerializeField] private float _defaultValue = 0f;
        [SerializeField] private float _value = 0f;
        public UnityEvent<float> OnValueChanged;

        private void OnEnable()
        {
            _value = _defaultValue;
        }
n        public float Value
        {
            get => _value;
            set => SetValue(value);
        }
n        public void SetValue(float newValue)
        {
            if (Mathf.Approximately(_value, newValue)) return;
            _value = newValue;
            OnValueChanged?.Invoke(_value);
        }
n        public override void ResetToDefault()
        {
            SetValue(_defaultValue);
        }
    }
}