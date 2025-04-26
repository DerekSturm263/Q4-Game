using UnityEditor;
using UnityEngine;

namespace Types.Miscellaneous
{
    [CustomPropertyDrawer(typeof(Any))]
    internal class AnyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty typeProp = property.FindPropertyRelative("_type");
            EditorGUI.PropertyField(position, typeProp);

            Rect valuePosition = new(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PrefixLabel(valuePosition, new("Value"));

            System.Type type = System.Type.GetType(typeProp.FindPropertyRelative("_typeName").stringValue);

            if (Helpers.TypeEditors.TypeDictionary.TryGetValue(type, out System.Func<Rect, SerializedProperty, object> action))
            {
                SerializedProperty propertyType = property.FindPropertyRelative("_propertyType");
                SerializedProperty value;

                bool isUnityObject = type.IsSubclassOf(typeof(Object));
                propertyType.enumValueIndex = (int)(isUnityObject ? Any.PropertyType.UnityObject : Any.PropertyType.CSharpObject);

                if (isUnityObject)
                {
                    value = property.FindPropertyRelative("_serializableValue").FindPropertyRelative("_item2");

                    try { action(valuePosition, value); }
                    catch { value.objectReferenceValue = Any.GetDefault(type) as Object; }
                }
                else
                {
                    value = property.FindPropertyRelative("_cSharpObjValue");

                    try { value.boxedValue = action(valuePosition, value); }
                    catch { value.boxedValue = Any.GetDefault(type); }
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(property) + 8;
        }
    }
}
