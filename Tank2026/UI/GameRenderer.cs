using System;
using System.Windows;
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
    private readonly Brush _playerTankBrush = SpriteBrushes.GetTankBrush(Brushes.Yellow);
    private readonly Brush _enemyBasicBrush = SpriteBrushes.GetTankBrush(Brushes.Silver);
    private readonly Brush _enemyFastBrush = SpriteBrushes.GetTankBrush(Brushes.LightPink);
    private readonly Brush _enemyPowerBrush = SpriteBrushes.GetTankBrush(Brushes.OrangeRed);
    private readonly Brush _enemyArmor4Brush = SpriteBrushes.GetTankBrush(Brushes.ForestGreen);
    private readonly Brush _enemyArmor3Brush = SpriteBrushes.GetTankBrush(Brushes.GreenYellow);
    private readonly Brush _enemyArmor2Brush = SpriteBrushes.GetTankBrush(Brushes.Yellow);
    private readonly Brush _powerupFlashBrush = SpriteBrushes.GetTankBrush(new SolidColorBrush(Color.FromRgb(255, 100, 100)));

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
            DrawTank(gameEngine.PlayerTank, _playerTankBrush);
        }

        var isFlashTick = DateTime.Now.Millisecond % 400 < 200;

        foreach (var enemy in gameEngine.Enemies)
        {
            if (enemy.IsAlive)
            {
                Brush brush = _enemyBasicBrush;
                if (enemy.IsFlashing && isFlashTick)
                {
                    brush = _powerupFlashBrush;
                }
                else
                {
                    brush = enemy.EnemyType switch
                    {
                        EnemyType.Fast => _enemyFastBrush,
                        EnemyType.Power => _enemyPowerBrush,
                        EnemyType.Armor => enemy.Health >= 4 ? _enemyArmor4Brush : enemy.Health == 3 ? _enemyArmor3Brush : enemy.Health == 2 ? _enemyArmor2Brush : _enemyBasicBrush,
                        _ => _enemyBasicBrush
                    };
                }
                DrawTank(enemy, brush);
            }
        }
        DrawBullets(gameEngine.Bullets);
        DrawExplosions(gameEngine.Explosions);
        DrawPowerups(gameEngine.Powerups);
    }

    private void DrawPowerups(System.Collections.Generic.IEnumerable<Powerup> powerups)
    {
        var isFlash = DateTime.Now.Millisecond % 400 < 200;
        foreach (var p in powerups)
        {
            var text = new TextBlock
            {
                Text = p.Type.ToString().Substring(0, 1),
                Foreground = isFlash ? Brushes.Red : Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 24
            };
            Canvas.SetLeft(text, p.X * GameSettings.TileSize + 8);
            Canvas.SetTop(text, p.Y * GameSettings.TileSize + 2);
            _canvas.Children.Add(text);
        }
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
        Brush fill = tileType switch
        {
            TileType.Brick => SpriteBrushes.BrickBrush,
            TileType.Steel => SpriteBrushes.SteelBrush,
            TileType.Water => SpriteBrushes.WaterBrush,
            TileType.Grass => SpriteBrushes.GrassBrush,
            TileType.Base => SpriteBrushes.BaseBrush,
            _ => Brushes.Transparent
        };

        if (fill == Brushes.Transparent) return;

        var rect = new Rectangle
        {
            Width = GameSettings.TileSize,
            Height = GameSettings.TileSize,
            Fill = fill
        };

        Canvas.SetLeft(rect, x * GameSettings.TileSize);
        Canvas.SetTop(rect, y * GameSettings.TileSize);
        _canvas.Children.Add(rect);
    }

    private void DrawTank(Tank tank, Brush brush)
    {
        var rect = new Rectangle
        {
            Width = GameSettings.TileSize,
            Height = GameSettings.TileSize,
            Fill = brush
        };

        var rotation = tank.Direction switch
        {
            Direction.Up => 0,
            Direction.Right => 90,
            Direction.Down => 180,
            Direction.Left => 270,
            _ => 0
        };

        if (rotation != 0)
        {
            rect.RenderTransformOrigin = new Point(0.5, 0.5);
            rect.RenderTransform = new RotateTransform(rotation);
        }

        Canvas.SetLeft(rect, tank.X * GameSettings.TileSize);
        Canvas.SetTop(rect, tank.Y * GameSettings.TileSize);
        _canvas.Children.Add(rect);
    }

    private void DrawBullets(System.Collections.Generic.IEnumerable<Bullet> bullets)
    {
        foreach (var bullet in bullets)
        {
            var size = GameSettings.TileSize / 4.0;
            var bulletShape = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = Brushes.Silver
            };

            var offset = (GameSettings.TileSize - size) / 2.0;
            Canvas.SetLeft(bulletShape, bullet.X * GameSettings.TileSize + offset);
            Canvas.SetTop(bulletShape, bullet.Y * GameSettings.TileSize + offset);
            _canvas.Children.Add(bulletShape);
        }
    }

    private void DrawExplosions(System.Collections.Generic.IEnumerable<Explosion> explosions)
    {
        foreach (var explosion in explosions)
        {
            var progress = explosion.RemainingTicks / (double)GameSettings.ExplosionDurationTicks;
            var size = GameSettings.TileSize * (0.6 + (1 - progress) * 0.4);

            var group = new DrawingGroup();
            group.Children.Add(new GeometryDrawing(Brushes.Red, null, new EllipseGeometry(new Point(0.5, 0.5), 0.5, 0.5)));
            group.Children.Add(new GeometryDrawing(Brushes.Yellow, null, new EllipseGeometry(new Point(0.5, 0.5), 0.3, 0.3)));
            group.Children.Add(new GeometryDrawing(Brushes.White, null, new EllipseGeometry(new Point(0.5, 0.5), 0.1, 0.1)));

            var flash = new Rectangle
            {
                Width = size,
                Height = size,
                Fill = new DrawingBrush(group) { Stretch = Stretch.Fill },
                Opacity = Math.Max(0, progress)
            };

            var left = explosion.X * GameSettings.TileSize + (GameSettings.TileSize - size) / 2;
            var top = explosion.Y * GameSettings.TileSize + (GameSettings.TileSize - size) / 2;
            Canvas.SetLeft(flash, left);
            Canvas.SetTop(flash, top);
            _canvas.Children.Add(flash);
        }
    }
}
