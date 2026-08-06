using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HelloDev.Variables.Editor
{
    [CustomEditor(typeof(ScriptableObject), true, isFallback = true)]
    public class VariableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var targetObj = target as ScriptableObject;
            var targetType = targetObj.GetType();

            // Try to find serialized properties for value and default
            var valueProp = serializedObject.FindProperty("_value") ?? serializedObject.FindProperty("value");
            var defaultProp = serializedObject.FindProperty("_defaultValue") ?? serializedObject.FindProperty("defaultValue");

            // Draw default value (editable)
            if (defaultProp != null)
            {
                EditorGUILayout.PropertyField(defaultProp, true);
            }

            EditorGUILayout.Space();

            // Draw current value as read-only
            if (valueProp != null)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(valueProp, new GUIContent("Value"), true);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                // Fallback: reflect and show Value property or _value field
                object currentVal = null;
                var pi = targetType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                {
                    currentVal = pi.GetValue(targetObj);
                }
                else
                {
                    var fi = targetType.GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    if (fi != null) currentVal = fi.GetValue(targetObj);
                }

                if (currentVal != null)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.LabelField("Value", currentVal.ToString());
                    EditorGUI.EndDisabledGroup();
                }
            }

            EditorGUILayout.Space();

            // Button to apply default value via SetValue/Value setter (invokes events)
            // Show if there's a default field or a Value setter / SetValue method.
            var fiDefault = targetType.GetField("_defaultValue", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            bool hasDefault = defaultProp != null || fiDefault != null;
            var piValue = targetType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            var hasSetter = (piValue != null && piValue.CanWrite) || targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Any(m => m.Name == "SetValue" && m.GetParameters().Length == 1);

            if (hasDefault && hasSetter)
            {
                if (GUILayout.Button("Apply Default (SetValue & Invoke)"))
                {
                    // Read default via field or serialized property
                    object defaultVal = null;
                    if (fiDefault != null) defaultVal = fiDefault.GetValue(targetObj);
                    else if (defaultProp != null)
                    {
                        // Attempt to get value from serialized property (less reliable for generics)
                        defaultVal = GetSerializedPropertyValue(defaultProp);
                    }

                    bool applied = false;

                    // Try property setter first
                    if (piValue != null && piValue.CanWrite)
                    {
                        piValue.SetValue(targetObj, defaultVal);
                        applied = true;
                    }
                    else
                    {
                        // Try SetValue method
                        var methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                        foreach (var m in methods)
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
                        // Mark dirty so asset change persists
                        EditorUtility.SetDirty(targetObj);
                        AssetDatabase.SaveAssets();
                    }
                }
            }

            // Reset button
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
    }
}
