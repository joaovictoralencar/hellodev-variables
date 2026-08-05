using System;
using UnityEngine;
using UnityEngine.Events;
using HelloDev.Utils;

namespace HelloDev.Variables
{
    /// <summary>
    /// Non-generic base for SO variables. Provides reset hook.
    /// </summary>
    public abstract class VariableBase_SO : RuntimeScriptableObject
    {
        public abstract void ResetToDefault();

        protected override void OnScriptableObjectReset()
        {
            ResetToDefault();
        }
    }
}