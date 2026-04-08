using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Tank2026.Core;
using Tank2026.Models;
using Tank2026.Services;

namespace Tank2026.UI;

public class GameRenderer
{
    private readonly Canvas _canvas;

    public GameRenderer(Canvas canvas)
    {
        _canvas = canvas;
    }

    public void Render(GameEngine gameEngine)
    {
        _canvas.Children.Clear();

        DrawMap(gameEngine.Map);
        if (gameEngine.PlayerTank.IsAlive)
        {
            DrawTank(gameEngine.PlayerTank, Brushes.DeepSkyBlue);
        }
        foreach (var enemy in gameEngine.Enemies)
        {
            if (enemy.IsAlive)
            {
                DrawTank(enemy, Brushes.OrangeRed);
            }
        }
        DrawBullets(gameEngine.Bullets);
        DrawExplosions(gameEngine.Explosions);
    }

    private void DrawMap(Map map)
    {
        for (var y = 0; y < map.Height; y++)
        {
            for (var x = 0; x < map.Width; x++)
            {
                var tile = map.GetTile(x, y);
                if (tile == TileType.Empty)
                {
                    continue;
                }

                DrawTile(x, y, tile);
            }
        }
    }

    private void DrawTile(int x, int y, TileType tileType)
    {
        switch (tileType)
        {
            case TileType.Brick:
                DrawCell(x, y, new SolidColorBrush(Color.FromRgb(156, 74, 36)), 0);
                DrawCell(x, y, new SolidColorBrush(Color.FromRgb(110, 45, 20)), 6);
                break;
            case TileType.Steel:
                DrawCell(x, y, new SolidColorBrush(Color.FromRgb(140, 140, 145)), 0);
                DrawCell(x, y, new SolidColorBrush(Color.FromRgb(95, 95, 100)), 8);
                break;
            case TileType.Water:
                DrawCell(x, y, new SolidColorBrush(Color.FromRgb(30, 120, 220)), 0);
                DrawCell(x, y, new SolidColorBrush(Color.FromArgb(120, 190, 230, 255)), 10);
                break;
            case TileType.Grass:
                DrawCell(x, y, new SolidColorBrush(Color.FromRgb(40, 140, 60)), 0);
                DrawCell(x, y, new SolidColorBrush(Color.FromRgb(60, 170, 80)), 12);
                break;
            case TileType.Base:
                DrawCell(x, y, new SolidColorBrush(Color.FromRgb(200, 190, 70)), 0);
                DrawCell(x, y, new SolidColorBrush(Color.FromRgb(80, 50, 30)), 10);
                break;
        }
    }

    private void DrawTank(Tank tank, Brush color)
    {
        var tankSize = GameSettings.TileSize - 4;
        var left = tank.X * GameSettings.TileSize + 2;
        var top = tank.Y * GameSettings.TileSize + 2;

        // Square tank body (retro look).
        var body = new Rectangle
        {
            Width = tankSize,
            Height = tankSize,
            Fill = color,
            Stroke = Brushes.Black,
            StrokeThickness = 1
        };
        Canvas.SetLeft(body, left);
        Canvas.SetTop(body, top);
        _canvas.Children.Add(body);

        var barrel = new Rectangle
        {
            Width = 6,
            Height = 14,
            Fill = Brushes.White
        };

        var centerX = left + tankSize / 2.0;
        var centerY = top + tankSize / 2.0;

        switch (tank.Direction)
        {
            case Direction.Up:
                Canvas.SetLeft(barrel, centerX - 3);
                Canvas.SetTop(barrel, top - 3);
                break;
            case Direction.Down:
                Canvas.SetLeft(barrel, centerX - 3);
                Canvas.SetTop(barrel, top + tankSize - 10);
                break;
            case Direction.Left:
                barrel.Width = 14;
                barrel.Height = 6;
                Canvas.SetLeft(barrel, left - 3);
                Canvas.SetTop(barrel, centerY - 3);
                break;
            case Direction.Right:
                barrel.Width = 14;
                barrel.Height = 6;
                Canvas.SetLeft(barrel, left + tankSize - 10);
                Canvas.SetTop(barrel, centerY - 3);
                break;
        }

        _canvas.Children.Add(barrel);
    }

    private void DrawBullets(IEnumerable<Bullet> bullets)
    {
        foreach (var bullet in bullets)
        {
            var size = GameSettings.TileSize / 3.0;
            var bulletShape = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = Brushes.Yellow
            };

            var offset = (GameSettings.TileSize - size) / 2.0;
            Canvas.SetLeft(bulletShape, bullet.X * GameSettings.TileSize + offset);
            Canvas.SetTop(bulletShape, bullet.Y * GameSettings.TileSize + offset);
            _canvas.Children.Add(bulletShape);
        }
    }

    private void DrawExplosions(IEnumerable<Explosion> explosions)
    {
        foreach (var explosion in explosions)
        {
            var progress = explosion.RemainingTicks / (double)GameSettings.ExplosionDurationTicks;
            var size = GameSettings.TileSize * (0.4 + (1 - progress) * 0.8);

            var flash = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = new SolidColorBrush(Color.FromArgb((byte)(200 * progress), 255, 170, 0))
            };

            var left = explosion.X * GameSettings.TileSize + (GameSettings.TileSize - size) / 2;
            var top = explosion.Y * GameSettings.TileSize + (GameSettings.TileSize - size) / 2;
            Canvas.SetLeft(flash, left);
            Canvas.SetTop(flash, top);
            _canvas.Children.Add(flash);
        }
    }

    private void DrawCell(int gridX, int gridY, Brush fill, double margin)
    {
        var rect = new Rectangle
        {
            Width = GameSettings.TileSize - margin,
            Height = GameSettings.TileSize - margin,
            Fill = fill
        };

        Canvas.SetLeft(rect, gridX * GameSettings.TileSize + margin / 2);
        Canvas.SetTop(rect, gridY * GameSettings.TileSize + margin / 2);
        _canvas.Children.Add(rect);
    }
}
