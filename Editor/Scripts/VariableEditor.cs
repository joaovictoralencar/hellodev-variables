using System.Reflection;
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
            if (defaultProp != null)
            {
                if (GUILayout.Button("Apply Default (SetValue & Invoke)"))
                {
                    // Read default via reflection for reliability
                    var fi = targetType.GetField("_defaultValue", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    object defaultVal = fi?.GetValue(targetObj);

                    bool applied = false;

                    // Try property setter first
                    var pi = targetType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
                    if (pi != null && pi.CanWrite)
                    {
                        pi.SetValue(targetObj, defaultVal);
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
                        Debug.Log($"Applied default value on {targetObj.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"Could not apply default value on {targetObj.name}: no setter or SetValue method found.");
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
    }
}
