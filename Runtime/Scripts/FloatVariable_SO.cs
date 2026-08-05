using UnityEngine;
using UnityEngine.Events;
using HelloDev.Utils;

namespace HelloDev.Variables
{
    [CreateAssetMenu(menuName = "HelloDev/Variables/Float Variable", fileName = "FloatVariable_SO")]
    public class FloatVariable_SO : Variable_SO<float>
    {
        protected override bool ValuesEqual(float a, float b)
        {
            return Mathf.Approximately(a, b);
        }
    }
}