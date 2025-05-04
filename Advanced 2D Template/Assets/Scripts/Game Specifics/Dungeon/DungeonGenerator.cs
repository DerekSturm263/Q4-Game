using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap _floorMap;
    [SerializeField] private Tilemap _objectMap;

    public void Generate(DungeonSettings settings)
    {
        GenerateRooms(settings);
        GenerateCorridors(settings);
    }

    private void GenerateRooms(DungeonSettings mapSettings)
    {
        var mapBounds = CreateMapBounds(mapSettings);
        int roomCount = Random.Range(mapSettings.RoomCount.Min, mapSettings.RoomCount.Max + 1);

        //_tilemap.BoxFill(Vector3Int.zero, mapSettings.Empty, 0, 0, bounds.size.x, bounds.size.y);

        for (int i = 0; i < roomCount; ++i)
        {
            GenerateRoom(mapBounds, mapSettings.Rooms[Random.Range(0, mapSettings.Rooms.Count)], mapSettings.Floor, mapSettings.Wall);
        }

        _floorMap.ResizeBounds();
        _objectMap.ResizeBounds();
    }

    private BoundsInt CreateMapBounds(DungeonSettings settings)
    {
        return new()
        {
            x = 0,
            y = 0,
            size = new
            (
                Random.Range(settings.Dimensions.Min.x, settings.Dimensions.Max.x + 1),
                Random.Range(settings.Dimensions.Min.y, settings.Dimensions.Max.y + 1),
                1
            )
        };
    }

    private void GenerateRoom(BoundsInt mapBounds, RoomSettings roomSettings, TileBase floor, TileBase wall)
    {
        var roomBounds = CreateRoomBounds(roomSettings, mapBounds);

        TileBase[] floorTiles = new TileBase[roomBounds.size.x * roomBounds.size.y];
        for (int x = 0; x < roomBounds.size.x; ++x)
        {
            for (int y = 0; y < roomBounds.size.y; ++y)
            {
                floorTiles[x + y * roomBounds.size.x] = floor;
            }
        }

        _floorMap.SetTilesBlock(roomBounds, floorTiles);

        TileBase[] wallTiles = new TileBase[roomBounds.size.x * roomBounds.size.y];
        for (int i = 0; i < wallTiles.Length; ++i)
        {
            wallTiles[i] = wall;
        }

        Vector3Int[] wallPositions = new Vector3Int[roomBounds.size.x * 2 + roomBounds.size.y * 2 - 4];
        for (int i = 0; i < wallPositions.Length; ++i)
        {
            wallPositions[i] = new(i % roomBounds.size.x, i / roomBounds.size.x);
        }

        _objectMap.SetTiles(wallPositions, wallTiles);
    }

    private BoundsInt CreateRoomBounds(RoomSettings settings, BoundsInt mapBounds)
    {
        Vector3Int roomSize = new
        (
            Random.Range(settings.Dimensions.Min.x, settings.Dimensions.Max.x + 1),
            Random.Range(settings.Dimensions.Min.y, settings.Dimensions.Max.y + 1),
            1
        );
        Vector3Int roomPosition = new
        (
            Random.Range(roomSize.x / 2, (mapBounds.size.x - roomSize.x / 2) + 1),
            Random.Range(roomSize.y / 2, (mapBounds.size.y - roomSize.x / 2) + 1),
            1
        );

        return new(roomPosition, roomSize);
    }

    private void GenerateCorridors(DungeonSettings mapSettings)
    {

    }

    private void GenerateCorridor(Vector2Int start, Vector2Int end, CorridorSettings mapSettings)
    {

    }
}
