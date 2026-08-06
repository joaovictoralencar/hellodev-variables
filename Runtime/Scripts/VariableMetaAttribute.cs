using System;
using UnityEngine;

namespace HelloDev.Variables
{
    /// <summary>
    /// Optional attribute to provide metadata used when auto-creating Variable SO assets via the inspector.
    /// Example: [VariableMeta("Currency","Coin","Player")] will produce asset name like:
    /// SO_Variable_Int_Currency_Coin_Player
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class VariableMetaAttribute : PropertyAttribute
    {
        public string Category { get; }
        public string ItemName { get; }
        public string Owner { get; }

        public VariableMetaAttribute(string category = "Generic", string itemName = "", string owner = "")
        {
            Category = category ?? "Generic";
            ItemName = itemName ?? string.Empty;
            Owner = owner ?? string.Empty;
        }
    }
}