using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Types.Miscellaneous
{
    [CustomPropertyDrawer(typeof(Type))]
    public class TypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PrefixLabel(position, label, EditorStyles.boldLabel);

            SerializedProperty isArray = property.FindPropertyRelative("_isArray");
            EditorGUI.PropertyField(position, isArray, new GUIContent(" ", "Check this box if the type should be an array"));

            SerializedProperty typeName = property.FindPropertyRelative("_typeName");
            if (string.IsNullOrEmpty(typeName.stringValue))
                typeName.stringValue = typeof(int).AssemblyQualifiedName;

            System.Type type = System.Type.GetType(typeName.stringValue);

            List<System.Type> typeList = Helpers.TypeEditors.TypeDictionary.Select(item => item.Key).ToList();
            int typeIndex = typeList.IndexOf(type);

            Rect typePosition = new(position.x + 200, position.y, position.width - 200, EditorGUIUtility.singleLineHeight);
            int selectedType = EditorGUI.Popup(typePosition, "", typeIndex, typeList.Select(item => item.Name).ToArray());

            if (selectedType != typeIndex)
            {
                typeName.stringValue = typeList[selectedType].AssemblyQualifiedName;
                type = System.Type.GetType(typeName.stringValue);
            }

            EditorGUI.EndProperty();
        }
    }
}
