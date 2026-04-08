namespace Tank2026.Models;

public enum PowerupType
{
    Grenade,
    Timer,
    Shovel,
    Star,
    Life
}

public class Powerup
{
    public int X { get; set; }
    public int Y { get; set; }
    public PowerupType Type { get; set; }
}
