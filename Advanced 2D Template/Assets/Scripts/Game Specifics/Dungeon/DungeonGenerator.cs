using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;

    public void Generate(DungeonSettings settings)
    {

    }

    private void GenerateRooms(DungeonSettings settings)
    {
        int roomCount = Random.Range(settings.RoomCount.Min, settings.RoomCount.Max + 1);

        Vector2Int dimensions = new
        (
            Random.Range(settings.Dimensions.Min.x, settings.Dimensions.Max.x + 1),
            Random.Range(settings.Dimensions.Min.y, settings.Dimensions.Max.y + 1)
        );

        _tilemap.size = (Vector3Int)dimensions;
        _tilemap.FloodFill(Vector3Int.zero, settings.Empty);

        // Generate first room.
        GenerateRoom();

        // Generate second room.
        GenerateRoom();

        for (int i = 0; i < roomCount - 2; ++i)
        {
            Vector2Int position = new
            (
                Random.Range(0, dimensions.x + 1),
                Random.Range(0, dimensions.y + 1)
            );

            GenerateRoom(position, )
        }
    }

    private void GenerateRoom(BoundsInt position, RoomSettings settings, TileBase floor, TileBase wall)
    {
        Vector2Int dimensions = new
        (
            Random.Range(settings.Dimensions.Min.x, settings.Dimensions.Max.x + 1),
            Random.Range(settings.Dimensions.Min.y, settings.Dimensions.Max.y + 1)
        );

        BoundsInt bounds = new()
        {
            min = (Vector3Int)(position - dimensions / 2),
            max = (Vector3Int)(position + dimensions / 2)
        };

        TileBase[] tiles = new TileBase[bounds.size.x * bounds.size.y];

        for (int x = 0; x < bounds.size.x; ++x)
        {
            for (int y = 0; y < bounds.size.y; ++y)
            {
                if (x == 0 || y == 0 || x == bounds.size.x - 1 || y == bounds.size.y - 1)
                {
                    tiles[x + y * bounds.size.x] = wall;
                }
                else
                {
                    tiles[x + y * bounds.size.x] = floor;
                }
            }
        }

        _tilemap.SetTilesBlock(bounds, tiles);
    }

    private void GenerateCorridors(DungeonSettings settings)
    {

    }

    private void GenerateCooridor(Vector2Int start, Vector2Int end, CorrdiorSettings settings)
    {

    }
}
