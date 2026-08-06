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

            // Draw all visible properties except the internal _value which should be read-only
            var prop = serializedObject.GetIterator();
            bool enterChildren = true;
            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (prop.name == "_value")
                {
                    // Draw read-only value field
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(prop, new GUIContent("Value"));
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    EditorGUILayout.PropertyField(prop, true);
                }
            }

            // Reset button
            if (target is HelloDev.Variables.VariableBase_SO varBase)
            {
                GUILayout.Space(6);
                if (GUILayout.Button("Reset To Default"))
                {
                    varBase.ResetToDefault();
                    if (!Application.isPlaying)
                    {
                        // Mark dirty so Unity saves the changed value in edit mode
                        EditorUtility.SetDirty(varBase);
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}