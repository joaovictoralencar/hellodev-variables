using UnityEngine;
using UnityEngine.Events;
using HelloDev.Utils;

namespace HelloDev.Variables
{
    [CreateAssetMenu(menuName = "HelloDev/Variables/String Variable", fileName = "StringVariable_SO")]
    public class StringVariable_SO : VariableBase_SO
    {
        [SerializeField] private string _defaultValue = string.Empty;
        [SerializeField] private string _value = string.Empty;
        public UnityEvent<string> OnValueChanged;
n        private void OnEnable()
        {
            _value = _defaultValue;
        }
n        public string Value
        {
            get => _value;
            set => SetValue(value);
        }
n        public void SetValue(string newValue)
        {
            if (string.Equals(_value, newValue)) return;
            _value = newValue;
            OnValueChanged?.Invoke(_value);
        }
n        public override void ResetToDefault()
        {
            SetValue(_defaultValue);
        }
    }
}