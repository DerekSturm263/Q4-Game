using Types.Miscellaneous;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(BattleAction))]
public class BattleActionEditor : UnityEditor.Editor
{
    private ReorderableList _events;

    public override void OnInspectorGUI()
    {
        BattleAction action = target as BattleAction;

        _events ??= new(serializedObject, serializedObject.FindProperty("_events"), true, true, true, true)
        {
            drawHeaderCallback = DrawHeader,
            drawElementCallback = DrawElement,
            elementHeightCallback = ElementHeight,
            onAddCallback = Add,
            onRemoveCallback = Remove
        };

        _events.DoLayoutList();
    }

    private void DrawHeader(Rect rect)
    {
        GUI.Label(rect, "Events");
    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        var editor = CreateEditorWithContext(new Object[] { (target as BattleAction).Events[index] }, target as BattleAction);
        var property = editor.serializedObject.GetIterator();

        editor.serializedObject.Update();

        bool enterChildren = true;
        while (property.NextVisible(enterChildren))
        {
            EditorGUI.PropertyField(rect, property, true);
            enterChildren = false;
            
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        editor.serializedObject.ApplyModifiedProperties();
    }

    private float ElementHeight(int index)
    {
        var editor = CreateEditorWithContext(new Object[] { (target as BattleAction).Events[index] }, target as BattleAction);
        var property = editor.serializedObject.GetIterator();
        property.NextVisible(true);

        return EditorGUIUtility.singleLineHeight * (property.CountRemaining() + 1) + EditorGUIUtility.singleLineHeight * 0.5f;
    }

    private void Add(ReorderableList list)
    {
        Rect rect = new(Event.current.mousePosition, new(500, 700));
        PopupWindow.Show(rect, new ActionEventPopupWindow(target as BattleAction));
    }

    private void Remove(ReorderableList list)
    {
        foreach (var index in list.selectedIndices)
        {
            DestroyImmediate((target as BattleAction).Events[index], true);
        }

        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssets();
        
        foreach (var index in list.selectedIndices)
        {
            list.serializedProperty.DeleteArrayElementAtIndex(index);
        }
    }
}
