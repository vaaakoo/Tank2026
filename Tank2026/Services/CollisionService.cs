using Tank2026.Models;

namespace Tank2026.Services;

public class CollisionService
{
    public bool CanMoveTo(Map map, int x, int y)
    {
        return map.IsWalkable(x, y);
    }

    public bool BulletShouldStop(Map map, int x, int y)
    {
        var tile = map.GetTile(x, y);
        return tile is TileType.Brick or TileType.Steel or TileType.Base;
    }

    public bool BulletDestroysTile(Map map, int x, int y)
    {
        return map.GetTile(x, y) is TileType.Brick or TileType.Base;
    }
}
