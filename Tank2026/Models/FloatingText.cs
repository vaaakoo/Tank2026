namespace Tank2026.Models;

public class FloatingText
{
    public double X { get; set; }
    public double Y { get; set; }
    public string Text { get; set; } = string.Empty;
    public int TicksRemaining { get; set; }
    public int MaxTicks { get; set; }
}
