using SingletonBehaviours;
using UnityEngine;
using UnityEngine.Events;

public class DisplayHealth : MonoBehaviours.UI.Display<float, UnityEvent<float>>
{
    [SerializeField] private int _index;

    public override void UpdateDisplay(float item) => _component.Invoke(item);

    protected override float GetValue() => SaveDataController.Instance.CurrentData.Stats[_index].CurrentHealth / SaveDataController.Instance.CurrentData.Stats[_index].MaxHealth;
}
