using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Card", menuName = "Game/Card")]
public class Card : ScriptableObject
{
    [TextArea][SerializeField] private string _description;
    [SerializeField] private UnityEvent _effect;
    [SerializeField] private Sprite _texture;

    public AttackResults Invoke()
    {
        return new();
    }
}
