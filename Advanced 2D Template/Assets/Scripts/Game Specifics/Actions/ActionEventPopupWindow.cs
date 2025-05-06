using UnityEditor;
using UnityEngine;

public class ActionEventPopupWindow : PopupWindowContent
{
    private BattleAction _action;

    public ActionEventPopupWindow(BattleAction action)
    {
        _action = action;
    }

    public override void OnGUI(Rect rect)
    {
        if (GUILayout.Button("Damage"))
            AddEvent<DamageEvent>();

        if (GUILayout.Button("Toggle Damage"))
            AddEvent<ToggleDamageEvent>();

        if (GUILayout.Button("Move"))
            AddEvent<MoveEvent>();

        if (GUILayout.Button("Wait"))
            AddEvent<WaitEvent>();

        if (GUILayout.Button("Choose Target"))
            AddEvent<ChooseTargetEvent>();
    }

    private void AddEvent<T>() where T : ActionEvent
    {
        var damageEvent = ScriptableObject.CreateInstance<T>();
        damageEvent.name = "New Event";

        AssetDatabase.AddObjectToAsset(damageEvent, _action);
        _action.Events.Add(damageEvent);

        AssetDatabase.SaveAssets();
    }
}
