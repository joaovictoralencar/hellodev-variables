using UnityEditor;
using UnityEngine;

namespace HelloDev.Variables.Editor
{
    [CustomEditor(typeof(ScriptableObject), true, isFallback = true)]
    public class VariableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

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
                    }
                }
            }
        }
    }
}