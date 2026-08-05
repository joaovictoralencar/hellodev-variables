using UnityEngine;
using UnityEngine.Events;
using HelloDev.Utils;

namespace HelloDev.Variables
{
    [CreateAssetMenu(menuName = "HelloDev/Variables/Bool Variable", fileName = "BoolVariable_SO")]
    public class BoolVariable_SO : VariableBase_SO
    {
        [SerializeField] private bool _defaultValue = false;
        [SerializeField] private bool _value = false;
        public UnityEvent<bool> OnValueChanged;
n        private void OnEnable()
        {
            _value = _defaultValue;
        }
n        public bool Value
        {
            get => _value;
            set => SetValue(value);
        }
n        public void SetValue(bool newValue)
        {
            if (_value == newValue) return;
            _value = newValue;
            OnValueChanged?.Invoke(_value);
        }
n        public override void ResetToDefault()
        {
            SetValue(_defaultValue);
        }
    }
}