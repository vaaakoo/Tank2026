using Tank2026.Core;

namespace Tank2026.Models;

public class Tank : GameObject
{
    public Direction Direction { get; set; } = Direction.Up;
    public bool IsPlayer { get; set; }
    public bool IsAlive { get; set; } = true;
    public int MoveCooldown { get; set; }
    public int ShootCooldown { get; set; }
}
