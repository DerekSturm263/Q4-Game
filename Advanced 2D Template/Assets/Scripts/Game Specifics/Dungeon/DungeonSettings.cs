using System.Collections.Generic;
using Types.Miscellaneous;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Dungeon Settings", menuName = "Game/Dungeon/Dungeon Settings")]
public class DungeonSettings : ScriptableObject
{
    [SerializeField] private Range<Vector2Int> _dimensions;
    public Range<Vector2Int> Dimensions => _dimensions;

    [SerializeField] private Range<int> _roomCount;
    public Range<int> RoomCount => _roomCount;

    [SerializeField] private Range<int> _roomSpacing;
    public Range<int> RoomSpacing => _roomSpacing;

    [SerializeField] private List<RoomSettings> _rooms;
    public List<RoomSettings> Rooms => _rooms;

    [SerializeField] private CorridorSettings _corridors;
    public CorridorSettings Corridors => _corridors;

    [SerializeField] private TileBase _wall;
    public TileBase Wall => _wall;
    
    [SerializeField] private TileBase _floor;
    public TileBase Floor => _floor;

    [SerializeField] private TileBase _empty;
    public TileBase Empty => _empty;

    [SerializeField] private List<TileBase> _decorations;
    public List<TileBase> Decorations => _decorations;
}
