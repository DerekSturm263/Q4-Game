using Types.Collections;
using Types.Miscellaneous;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Card", menuName = "Game/Card")]
public class Card : ScriptableObject
{
    [TextArea][SerializeField] private string _description;
    [SerializeField] private Sprite _texture;

    [SerializeField] private UnityEvent<AttackInfo> _effect;
    public UnityEvent<AttackInfo> Effect => _effect;

    [SerializeField] private Dictionary<string, Any> _data;

    public void Invoke(AttackInfo info)
    {

    }

    public void DealDamage(AttackInfo info)
    {
        if (_data.TryGetValue("Damage", out Any value))
            info.defender.GetStats().ModifyHealth(value.Get<float>());
    }
}
