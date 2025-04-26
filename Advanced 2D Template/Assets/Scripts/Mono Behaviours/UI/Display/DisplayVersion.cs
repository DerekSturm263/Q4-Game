using UnityEngine;
using UnityEngine.Events;

public class DisplayVersion : MonoBehaviours.UI.Display<string, UnityEvent<string>>
{
    public override void UpdateDisplay(string item) => _component.Invoke(item);

    protected override string GetValue() => Application.version;
}
