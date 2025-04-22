using Types.Miscellaneous;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Game/Quest")]
public class Quest : ScriptableObject
{
    [TextArea][SerializeField] private string _description;
    public string Description => _description;
}
