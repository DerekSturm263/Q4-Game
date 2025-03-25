using Types.Collections;
using Types.Miscellaneous;
using UnityEngine;
using UnityEngine.Events;

public abstract class Item : ScriptableObject
{
    [TextArea][SerializeField] private string _description;
    [SerializeField] private Sprite _texture;
}
