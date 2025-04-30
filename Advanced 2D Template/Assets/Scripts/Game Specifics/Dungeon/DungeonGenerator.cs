using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;

    public void Generate(DungeonSettings settings)
    {
        GenerateRooms(settings);
        GenerateCorridors(settings);
    }

    private void GenerateRooms(DungeonSettings mapSettings)
    {
        int roomCount = Random.Range(mapSettings.RoomCount.Min, mapSettings.RoomCount.Max + 1);

        BoundsInt bounds = new()
        {
            x = 0,
            y = 0,
            size = new
            (
                Random.Range(mapSettings.Dimensions.Min.x, mapSettings.Dimensions.Max.x + 1),
                Random.Range(mapSettings.Dimensions.Min.y, mapSettings.Dimensions.Max.y + 1)
            )
        };

        _tilemap.size = bounds.size;
        _tilemap.FloodFill(Vector3Int.zero, mapSettings.Empty);

        for (int i = 0; i < roomCount; ++i)
        {
            GenerateRoom(bounds, mapSettings.Rooms[Random.Range(0, mapSettings.Rooms.Count)], mapSettings.Floor, mapSettings.Wall);
        }
    }

    private void GenerateRoom(BoundsInt mapBounds, RoomSettings roomSettings, TileBase floor, TileBase wall)
    {
        Vector2Int dimensions = new
        (
            Random.Range(roomSettings.Dimensions.Min.x, roomSettings.Dimensions.Max.x + 1),
            Random.Range(roomSettings.Dimensions.Min.y, roomSettings.Dimensions.Max.y + 1)
        );

        Vector2Int position = new
        (
            Random.Range(dimensions.x / 2, (mapBounds.size.x - dimensions.x / 2) + 1),
            Random.Range(dimensions.y / 2, (mapBounds.size.y - dimensions.x / 2) + 1)
        );

        BoundsInt roomBounds = new()
        {
            min = (Vector3Int)(position - dimensions / 2),
            max = (Vector3Int)(position + dimensions / 2)
        };

        TileBase[] tiles = new TileBase[roomBounds.size.x * roomBounds.size.y];

        for (int x = 0; x < roomBounds.size.x; ++x)
        {
            for (int y = 0; y < roomBounds.size.y; ++y)
            {
                if (x == 0 || y == 0 || x == roomBounds.size.x - 1 || y == roomBounds.size.y - 1)
                {
                    tiles[x + y * roomBounds.size.x] = wall;
                }
                else
                {
                    tiles[x + y * roomBounds.size.x] = floor;
                }
            }
        }

        _tilemap.SetTilesBlock(roomBounds, tiles);
    }

    private void GenerateCorridors(DungeonSettings mapSettings)
    {

    }

    private void GenerateCorridor(Vector2Int start, Vector2Int end, CorridorSettings mapSettings)
    {

    }
}
