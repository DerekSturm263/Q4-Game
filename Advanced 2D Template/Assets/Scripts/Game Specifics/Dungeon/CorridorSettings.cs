using Types.Miscellaneous;
using UnityEngine;

[CreateAssetMenu(fileName = "New Corridor Settings", menuName = "Game/Dungeon/Corridor Settings")]
public class CorridorSettings : ScriptableObject
{
    public enum CorridorType
    {
        Staircase,
        StraightLine,
        Curvy
    }

    [SerializeField] private Range<int> _thickness;
    public Range<int> Thickness => _thickness;

    [SerializeField] private CorridorType _type;
    public CorridorType Type => _type;
}
