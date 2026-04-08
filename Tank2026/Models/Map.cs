using Tank2026.Core;

namespace Tank2026.Models;

public class Map
{
    private readonly TileType[,] _grid;

    public int Width { get; }
    public int Height { get; }

    public Map(int width, int height)
    {
        Width = width;
        Height = height;
        _grid = new TileType[height, width];
    }

    public static Map CreateDefault()
    {
        var map = new Map(GameSettings.MapWidth, GameSettings.MapHeight);

        for (var x = 0; x < map.Width; x++)
        {
            map.SetTile(x, 0, TileType.Steel);
            map.SetTile(x, map.Height - 1, TileType.Steel);
        }

        for (var y = 0; y < map.Height; y++)
        {
            map.SetTile(0, y, TileType.Steel);
            map.SetTile(map.Width - 1, y, TileType.Steel);
        }

        for (var x = 2; x < map.Width - 2; x += 3)
        {
            for (var y = 2; y < map.Height - 2; y += 2)
            {
                map.SetTile(x, y, TileType.Brick);
            }
        }

        // Central river strip: blocks tanks, bullets pass over.
        for (var y = 3; y < map.Height - 3; y++)
        {
            map.SetTile(map.Width / 2, y, TileType.Water);
        }

        // Decorative grass that tanks can move through.
        for (var x = 3; x < map.Width - 3; x += 5)
        {
            for (var y = 1; y < map.Height - 1; y += 4)
            {
                if (map.GetTile(x, y) == TileType.Empty)
                {
                    map.SetTile(x, y, TileType.Grass);
                }
            }
        }

        // Keep player/enemy spawn corridors clear.
        map.SetTile(1, map.Height - 2, TileType.Empty);
        map.SetTile(1, map.Height - 3, TileType.Empty);
        map.SetTile(2, map.Height - 2, TileType.Empty);
        map.SetTile(map.Width - 2, 1, TileType.Empty);
        map.SetTile(map.Width - 3, 1, TileType.Empty);
        map.SetTile(map.Width - 2, 2, TileType.Empty);

        // Build a classic castle area near bottom center.
        var baseX = map.Width / 2;
        var baseY = map.Height - 2;
        map.SetTile(baseX, baseY, TileType.Base);
        for (var y = baseY - 1; y <= baseY; y++)
        {
            for (var x = baseX - 1; x <= baseX + 1; x++)
            {
                if (x == baseX && y == baseY)
                {
                    continue;
                }

                map.SetTile(x, y, TileType.Brick);
            }
        }

        return map;
    }

    public bool InBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public TileType GetTile(int x, int y)
    {
        if (!InBounds(x, y))
        {
            return TileType.Steel;
        }

        return _grid[y, x];
    }

    public void SetTile(int x, int y, TileType tileType)
    {
        if (!InBounds(x, y))
        {
            return;
        }

        _grid[y, x] = tileType;
    }

    public bool IsWalkable(int x, int y)
    {
        if (!InBounds(x, y))
        {
            return false;
        }

        var tile = GetTile(x, y);
        return tile is TileType.Empty or TileType.Grass;
    }
}
