using UnityEngine;

public abstract class Item : ScriptableObject
{
    [TextArea][SerializeField] private string _description;
    public string Description => _description;

    [SerializeField] private Sprite _texture;
    public Sprite Texture => _texture;
}
