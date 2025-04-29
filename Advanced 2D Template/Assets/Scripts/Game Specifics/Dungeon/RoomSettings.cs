using Types.Miscellaneous;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room Settings", menuName = "Game/Dungeon/Room Settings")]
public class RoomSettings : ScriptableObject
{
    [SerializeField] private Range<Vector2Int> _dimensions;
    public Range<Vector2Int> Dimensions => _dimensions;
}
