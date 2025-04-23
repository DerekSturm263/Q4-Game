using UnityEngine;

public abstract class Item : ScriptableObject
{
    [SerializeField] private Sprite _icon;
    public Sprite Icon => _icon;

    [TextArea][SerializeField] private string _description;
    public string Description => _description;
}
