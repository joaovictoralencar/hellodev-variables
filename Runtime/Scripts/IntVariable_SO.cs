using UnityEngine;
using UnityEngine.Events;
using HelloDev.Utils;

namespace HelloDev.Variables
{
    [CreateAssetMenu(menuName = "HelloDev/Variables/Int Variable", fileName = "IntVariable_SO")]
    public class IntVariable_SO : VariableBase_SO
    {
        [SerializeField] private int _defaultValue = 0;
        [SerializeField] private int _value = 0;
        public UnityEvent<int> OnValueChanged;
n        private void OnEnable()
        {
            _value = _defaultValue;
        }
n        public int Value
        {
            get => _value;
            set => SetValue(value);
        }
n        public void SetValue(int newValue)
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