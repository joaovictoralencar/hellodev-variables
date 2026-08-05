using UnityEngine;
using UnityEngine.Events;
using HelloDev.Utils;

namespace HelloDev.Variables
{
    /// <summary>
    /// Generic base class for ScriptableObject variables of any type.
    /// Provides automatic value tracking and change events.
    /// </summary>
    /// <typeparam name="T">The type of value stored in this variable.</typeparam>
    /// <remarks>
    /// Inherit from this class to create typed variables. For built-in types,
    /// use the provided *Variable_SO classes directly or use this for custom types.
    /// </remarks>
    public abstract class Variable_SO<T> : VariableBase_SO
    {
        [SerializeField] protected T _defaultValue;
        [SerializeField] protected T _value;

        public UnityEvent<T> OnValueChanged;

        protected virtual void OnEnable()
        {
            if (_value == null && _defaultValue == null)
            {
                _value = _defaultValue;
            }
        }

        /// <summary>
        /// Gets or sets the current value, triggering OnValueChanged if it changes.
        /// </summary>
        public T Value
        {
            get => _value;
            set => SetValue(value);
        }

        /// <summary>
        /// Sets the value if it differs from the current value.
        /// Invokes OnValueChanged if a change occurred.
        /// </summary>
        public virtual void SetValue(T newValue)
        {
            if (ValuesEqual(_value, newValue))
                return;

            _value = newValue;
            OnValueChanged?.Invoke(_value);
        }

        /// <summary>
        /// Resets the value to default and invokes OnValueChanged.
        /// </summary>
        public override void ResetToDefault()
        {
            SetValue(_defaultValue);
        }

        /// <summary>
        /// Override to customize value comparison logic.
        /// Default: uses EqualityComparer<T>.Default
        /// </summary>
        protected virtual bool ValuesEqual(T a, T b)
        {
            return System.Collections.Generic.EqualityComparer<T>.Default.Equals(a, b);
        }
    }
}