using UnityEngine;
using UnityEditor;
using System.IO;

namespace HelloDev.Variables.Editor
{
    /// <summary>
    /// PropertyDrawer for VariableBase_SO fields.
    /// Shows a "Create" button when the field is null, allowing quick SO creation.
    /// When a value is already assigned the full width is used.
    /// </summary>
    [CustomPropertyDrawer(typeof(VariableBase_SO), useForChildren: true)]
    public class VariablePropertyDrawer : PropertyDrawer
    {
        private const string DefaultVariablesFolder = "Assets/Variables";
        private const float ButtonWidth = 70f;
        private const float ButtonPadding = 4f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            bool isNull = property.objectReferenceValue == null;

            if (isNull)
            {
                // Reserve space for the Create button
                Rect propertyRect = new Rect(position.x, position.y, position.width - (ButtonWidth + ButtonPadding), position.height);
                EditorGUI.PropertyField(propertyRect, property, label);

                Rect buttonRect = new Rect(position.x + position.width - ButtonWidth, position.y, ButtonWidth, position.height);
                if (GUI.Button(buttonRect, "Create", EditorStyles.miniButton))
                    CreateVariableAsset(property);
            }
            else
            {
                // Full width when a variable is assigned
                EditorGUI.PropertyField(position, property, label);
            }

            EditorGUI.EndProperty();
        }

        private void CreateVariableAsset(SerializedProperty property)
        {
            // Determine the variable type from the field's actual type
            var fieldType = fieldInfo.FieldType;

            if (fieldType.IsArray)
                fieldType = fieldType.GetElementType();

            // If it's a generic type (e.g., Variable_SO<T>), attempt to get the generic argument
            if (fieldType.IsGenericType)
            {
                var args = fieldType.GetGenericArguments();
                if (args != null && args.Length > 0)
                            {
                                var genArg = args[0];
                                if (genArg.IsSubclassOf(typeof(ScriptableObject))) fieldType = genArg;
                            }
                        }

            // Resolve the SO variable concrete type (handles common typed classes)
            // If fieldType is a subclass of ScriptableObject already (e.g., FloatVariable_SO), use it
            if (!typeof(ScriptableObject).IsAssignableFrom(fieldType))
            {
                // Fallback: try to find a Variable type in the assembly matching the field type name
                // Try to find candidate ScriptableObject types whose name contains the field type name
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                System.Type found = null;
                foreach (var a in assemblies)
                {
                    System.Type[] types = null;
                    try { types = a.GetTypes(); } catch { continue; }
                    foreach (var t in types)
                    {
                        if (typeof(ScriptableObject).IsAssignableFrom(t) && t.Name.IndexOf(fieldType.Name, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            found = t;
                            break;
                        }
                    }
                    if (found != null) break;
                }
                if (found != null) fieldType = found;
            }

            // Create folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder(DefaultVariablesFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Variables");
            }

            // Determine name components
            string typeSegment = InferTypeSegment(fieldType);
            string category = "Generic";
            string itemName = SanitizeName(property.name);
            string owner = SanitizeName(property.serializedObject.targetObject.name);

            // Check for VariableMeta attribute on the field
            HelloDev.Variables.VariableMetaAttribute metaAttr = null;
            var rawAttrs = fieldInfo.GetCustomAttributes(typeof(HelloDev.Variables.VariableMetaAttribute), false);
            if (rawAttrs != null)
            {
                for (int i = 0; i < rawAttrs.Length; i++)
                {
                    if (rawAttrs[i] is HelloDev.Variables.VariableMetaAttribute vma)
                    {
                        metaAttr = vma;
                        break;
                    }
                }
            }

            if (metaAttr != null)
            {
                category = string.IsNullOrEmpty(metaAttr.Category) ? category : SanitizeName(metaAttr.Category);
                if (!string.IsNullOrEmpty(metaAttr.ItemName)) itemName = SanitizeName(metaAttr.ItemName);
                if (!string.IsNullOrEmpty(metaAttr.Owner)) owner = SanitizeName(metaAttr.Owner);
            }

            // Build filename using requested pattern
            var candidateParts = new[] { "SO", "Variable", typeSegment, category, itemName, owner };
            var partsList = new System.Collections.Generic.List<string>(candidateParts.Length);
            for (int i = 0; i < candidateParts.Length; i++)
            {
                var p = candidateParts[i];
                if (!string.IsNullOrEmpty(p)) partsList.Add(p);
            }
            var parts = partsList.ToArray();

            string fileName = string.Join("_", parts);
            string assetPath = Path.Combine(DefaultVariablesFolder, fileName + ".asset");
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
        }

        private string InferTypeSegment(System.Type fieldType)
        {
            if (fieldType == null) return "Unknown";
            string name = fieldType.Name;

            // Common typed classes naming (FloatVariable_SO -> Float)
            if (name.EndsWith("Variable_SO"))
                return name.Replace("Variable_SO", "");

            // If generic like Variable_SO`1, try to read generic arg name
            if (fieldType.IsGenericType)
            {
                var args = fieldType.GetGenericArguments();
                if (args != null && args.Length > 0) return args[0].Name;
            }

            // Fallback: strip common suffixes
            return name.Replace("Variable", string.Empty).Replace("_SO", string.Empty);
        }

        private string SanitizeName(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return string.Empty;
            // Replace spaces and illegal path chars with underscore
            var invalid = Path.GetInvalidFileNameChars();
            var chars = new System.Text.StringBuilder();
            foreach (var c in raw)
            {
                bool isInvalid = false;
                for (int i = 0; i < invalid.Length; i++) if (invalid[i] == c) { isInvalid = true; break; }
                chars.Append(isInvalid ? '_' : c);
            }
            var cleaned = chars.ToString();
            return cleaned.Replace('.', '_');
        }
    }
}