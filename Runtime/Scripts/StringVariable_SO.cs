using UnityEngine;
using UnityEngine.Events;
using HelloDev.Utils;

namespace HelloDev.Variables
{
    [CreateAssetMenu(menuName = "HelloDev/Variables/String Variable", fileName = "StringVariable_SO")]
    public class StringVariable_SO : Variable_SO<string>
    {
        protected override bool ValuesEqual(string a, string b)
        {
            return string.Equals(a, b);
        }
    }
}