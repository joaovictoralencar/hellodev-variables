using UnityEngine;
using UnityEditor;
using System.IO;

namespace HelloDev.Variables.Editor
{
    /// <summary>
    /// PropertyDrawer for VariableBase_SO fields.
    /// Shows a "Create" button when the field is null, allowing quick SO creation.
    /// </summary>
    [CustomPropertyDrawer(typeof(VariableBase_SO), useForChildren: true)]
    public class VariablePropertyDrawer : PropertyDrawer
    {
        private const string DefaultVariablesFolder = "Assets/Variables";
        private const float ButtonWidth = 60f;
        private const float ButtonPadding = 4f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw the property field (object reference)
            Rect propertyRect = new Rect(position.x, position.y, position.width - (ButtonWidth + ButtonPadding), position.height);
            EditorGUI.PropertyField(propertyRect, property, label);

            // Draw "Create" button only if the field is null
            if (property.objectReferenceValue == null)
            {
                Rect buttonRect = new Rect(position.x + position.width - ButtonWidth, position.y, ButtonWidth, position.height);
                if (GUI.Button(buttonRect, "Create", EditorStyles.miniButton))
                {
                    CreateVariableAsset(property);
                }
            }

            EditorGUI.EndProperty();
        }

        private void CreateVariableAsset(SerializedProperty property)
        {
            // Determine the variable type from the field's actual type
            var fieldType = fieldInfo.FieldType;

            // If it's a generic type, get the generic argument (e.g., List<FloatVariable_SO> -> FloatVariable_SO)
            if (fieldType.IsGenericType)
            {
                fieldType = fieldType.GetGenericArguments()[0];
            }

            // Create folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder(DefaultVariablesFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Variables");
            }

            // Generate a unique filename based on the field name and type
            string varTypeName = fieldType.Name.Replace("Variable_SO", "");
            string fileName = $"{property.serializedObject.targetObject.name}_{varTypeName}_Variable_SO";
            string assetPath = Path.Combine(DefaultVariablesFolder, $"{fileName}.asset");

            // Ensure unique filename
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

            // Create instance and save asset
            ScriptableObject newVariable = ScriptableObject.CreateInstance(fieldType);
            if (newVariable == null)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to create instance of type {fieldType.Name}", "OK");
                return;
            }

            AssetDatabase.CreateAsset(newVariable, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Assign the created asset to the property
            property.objectReferenceValue = newVariable;
            property.serializedObject.ApplyModifiedProperties();

            EditorUtility.DisplayDialog("Success", $"Created: {Path.GetFileName(assetPath)}", "OK");
        }
    }
}