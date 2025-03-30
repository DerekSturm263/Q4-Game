using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Types.Collections
{
    [CustomPropertyDrawer(typeof(Dictionary<,>))]
    internal class DictionaryDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty kvpsProperty = property.FindPropertyRelative("_kvps");
            ReorderableList list = new(property.serializedObject, kvpsProperty, true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) => DrawHeader(rect, property),
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => DrawElement(rect, kvpsProperty.GetArrayElementAtIndex(index)),
                elementHeightCallback = (int index) => ElementHeight(kvpsProperty.GetArrayElementAtIndex(index)),
                onAddCallback = (ReorderableList list) => OnAdd(kvpsProperty)
            };

            list.DoList(position);

            EditorGUI.EndProperty();
        }

        private void DrawHeader(Rect rect, SerializedProperty kvps)
        {
            GUI.Label(rect, kvps.displayName);
        }

        private void DrawElement(Rect rect, SerializedProperty kvp)
        {
            EditorGUI.PropertyField(new Rect(rect.position, new Vector2(rect.width, EditorGUIUtility.singleLineHeight)), kvp);
        }

        private float ElementHeight(SerializedProperty kvp)
        {
            return EditorGUI.GetPropertyHeight(kvp) + 2;
        }

        private void OnAdd(SerializedProperty kvps)
        {
            kvps.InsertArrayElementAtIndex(kvps.arraySize);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty kvpsProperty = property.FindPropertyRelative("_kvps");

            float size = 0;
            if (kvpsProperty.arraySize > 0)
            {
                for (int i = 0; i < kvpsProperty.arraySize; ++i)
                {
                    size += EditorGUI.GetPropertyHeight(kvpsProperty.GetArrayElementAtIndex(i)) + 4;
                }
            }
            else
            {
                size = EditorGUIUtility.singleLineHeight;
            }

            return size + EditorGUIUtility.singleLineHeight * 5f;
        }
    }
}
