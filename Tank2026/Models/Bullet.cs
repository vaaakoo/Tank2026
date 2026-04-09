using Tank2026.Core;

namespace Tank2026.Models;

public class Bullet : GameObject
{
    public Direction Direction { get; set; } = Direction.Up;
    public bool IsActive { get; set; } = true;
    public bool FiredByPlayer { get; set; }
    public int MoveCooldown { get; set; }
    public int MaxMoveCooldown { get; set; }
}
