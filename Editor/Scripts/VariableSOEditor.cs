#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HelloDev.Variables.Editor
{
[CustomEditor(typeof(HelloDev.Variables.VariableBase_SO), true)]
#if ODIN_INSPECTOR
    public class VariableSOEditor : OdinEditor
#else
    public class VariableSOEditor : UnityEditor.Editor
#endif
    {
#if !ODIN_INSPECTOR
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var targetObj = target as ScriptableObject;
            var targetType = targetObj.GetType();

            var defaultProp = serializedObject.FindProperty("_defaultValue") ?? serializedObject.FindProperty("defaultValue");
            var valueProp = serializedObject.FindProperty("_value") ?? serializedObject.FindProperty("value");

            // Draw default
            if (defaultProp != null)
                EditorGUILayout.PropertyField(defaultProp, true);

            EditorGUILayout.Space();

            // Draw value read-only
            if (valueProp != null)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(valueProp, new GUIContent("Value"), true);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                // fallback reflect Value
                var pi = targetType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                {
                    var val = pi.GetValue(targetObj);
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.LabelField("Value", val != null ? val.ToString() : "<null>");
                    EditorGUI.EndDisabledGroup();
                }
            }

            EditorGUILayout.Space();

            // Apply default button
            var fiDefault = targetType.GetField("_defaultValue", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            bool hasDefault = defaultProp != null || fiDefault != null;
            var piValue = targetType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            bool hasSetter = (piValue != null && piValue.CanWrite) || MethodExists(targetType, "SetValue", 1);

            if (hasDefault && hasSetter)
            {
                if (GUILayout.Button("Apply Default (SetValue & Invoke)"))
                {
                    object defaultVal = null;
                    if (fiDefault != null) defaultVal = fiDefault.GetValue(targetObj);
                    else if (defaultProp != null) defaultVal = GetSerializedPropertyValue(defaultProp);

                    bool applied = false;
                    if (piValue != null && piValue.CanWrite)
                    {
                        piValue.SetValue(targetObj, defaultVal);
                        applied = true;
                    }
                    else
                    {
                        foreach (var m in targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                        {
                            if (m.Name == "SetValue" && m.GetParameters().Length == 1)
                            {
                                m.Invoke(targetObj, new object[] { defaultVal });
                                applied = true;
                                break;
                            }
                        }
                    }

                    if (applied)
                    {
                        EditorUtility.SetDirty(targetObj);
                        AssetDatabase.SaveAssets();
                    }
                }
            }

            // Reset button
            if (target is HelloDev.Variables.VariableBase_SO varBase)
            {
                GUILayout.Space(4);
                if (GUILayout.Button("Reset To Default"))
                {
                    varBase.ResetToDefault();
                    if (!Application.isPlaying)
                    {
                        EditorUtility.SetDirty(varBase);
                        serializedObject.ApplyModifiedProperties();
                        AssetDatabase.SaveAssets();
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
#endif

        private object GetSerializedPropertyValue(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return prop.intValue;
                case SerializedPropertyType.Boolean:
                    return prop.boolValue;
                case SerializedPropertyType.Float:
                    return prop.floatValue;
                case SerializedPropertyType.String:
                    return prop.stringValue;
                case SerializedPropertyType.Color:
                    return prop.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return prop.objectReferenceValue;
                case SerializedPropertyType.Enum:
                    return prop.enumValueIndex;
                case SerializedPropertyType.Vector2:
                    return prop.vector2Value;
                case SerializedPropertyType.Vector3:
                    return prop.vector3Value;
                case SerializedPropertyType.Vector4:
                    return prop.vector4Value;
                case SerializedPropertyType.Rect:
                    return prop.rectValue;
                case SerializedPropertyType.AnimationCurve:
                    return prop.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return prop.boundsValue;
                default:
                    return null;
            }
        }

        private bool MethodExists(System.Type targetType, string methodName, int paramCount)
        {
            var methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var m in methods)
            {
                if (m.Name == methodName && m.GetParameters().Length == paramCount)
                    return true;
            }
            return false;
        }
    }
}
