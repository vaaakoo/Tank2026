using System.Windows.Threading;
using Tank2026.Core;
using Tank2026.Models;

namespace Tank2026.Services;

public class GameEngine
{
    private readonly DispatcherTimer _timer;
    private readonly CollisionService _collisionService;
    private readonly Random _random = new();
    private readonly (int x, int y)[] _enemySpawnPoints;
    private int _playerShootCooldown;
    private int _enemiesSpawnedTotal;

    public Map Map { get; }
    public Tank PlayerTank { get; }
    public List<Tank> Enemies { get; } = [];
    public List<Bullet> Bullets { get; } = [];
    public List<Explosion> Explosions { get; } = [];
    public bool IsGameOver { get; private set; }
    public bool IsPaused { get; private set; }
    public string StatusText { get; private set; } = "Battle started";
    public int PlayerLives { get; private set; } = GameSettings.PlayerStartingLives;
    public int Score { get; private set; }
    public int EnemyKills { get; private set; }

    public event Action? GameUpdated;

    public GameEngine(CollisionService collisionService)
    {
        _collisionService = collisionService;
        Map = Map.CreateDefault();
        _enemySpawnPoints =
        [
            (1, 1),
            (Map.Width / 2, 1),
            (Map.Width - 2, 1)
        ];

        PlayerTank = new Tank
        {
            IsPlayer = true,
            X = 1,
            Y = Map.Height - 2,
            Direction = Direction.Up
        };

        SpawnEnemiesIfNeeded();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(GameSettings.UpdateIntervalMs)
        };
        _timer.Tick += OnTick;
        UpdateHudText();
    }

    public void Start() => _timer.Start();

    public void Stop() => _timer.Stop();

    public void MovePlayer(Direction direction)
    {
        if (IsGameOver || IsPaused || !PlayerTank.IsAlive)
        {
            return;
        }

        PlayerTank.Direction = direction;
        MoveTankIfPossible(PlayerTank);
    }

    public void PlayerShoot()
    {
        if (IsGameOver || IsPaused || !PlayerTank.IsAlive || _playerShootCooldown > 0)
        {
            return;
        }

        _playerShootCooldown = TryFireFromTank(PlayerTank, _playerShootCooldown, 2);
    }

    public void Restart()
    {
        Bullets.Clear();
        Explosions.Clear();
        MapResetToDefault();

        PlayerTank.X = 1;
        PlayerTank.Y = Map.Height - 2;
        PlayerTank.Direction = Direction.Up;
        PlayerTank.IsAlive = true;

        Enemies.Clear();
        _enemiesSpawnedTotal = 0;
        SpawnEnemiesIfNeeded();

        PlayerLives = GameSettings.PlayerStartingLives;
        Score = 0;
        EnemyKills = 0;
        IsGameOver = false;
        IsPaused = false;
        _playerShootCooldown = 0;

        UpdateHudText();
        GameUpdated?.Invoke();
    }

    public void TogglePause()
    {
        if (IsGameOver)
        {
            return;
        }

        IsPaused = !IsPaused;
        StatusText = IsPaused ? "Paused | P to resume" : BuildHudText();
        GameUpdated?.Invoke();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (IsGameOver || IsPaused)
        {
            GameUpdated?.Invoke();
            return;
        }

        if (_playerShootCooldown > 0)
        {
            _playerShootCooldown--;
        }

        UpdateEnemyActions();
        UpdateBullets();
        UpdateExplosions();
        SpawnEnemiesIfNeeded();
        UpdateHudText();
        GameUpdated?.Invoke();
    }

    private void UpdateEnemyActions()
    {
        for (var i = Enemies.Count - 1; i >= 0; i--)
        {
            var enemy = Enemies[i];
            if (!enemy.IsAlive)
            {
                Enemies.RemoveAt(i);
                continue;
            }

            if (enemy.MoveCooldown > 0)
            {
                enemy.MoveCooldown--;
            }
            else
            {
                enemy.Direction = SelectEnemyDirection(enemy);
                MoveTankIfPossible(enemy);
                enemy.MoveCooldown = 2;
            }

            if (enemy.ShootCooldown > 0)
            {
                enemy.ShootCooldown--;
            }
            else if (_random.Next(0, 100) < 24)
            {
                enemy.ShootCooldown = TryFireFromTank(enemy, enemy.ShootCooldown, 4);
            }
        }
    }

    private void MoveTankIfPossible(Tank tank)
    {
        var next = GetNextPosition(tank.X, tank.Y, tank.Direction);
        if (_collisionService.CanMoveTo(Map, next.x, next.y) && !IsOtherTankOnCell(tank, next.x, next.y))
        {
            tank.X = next.x;
            tank.Y = next.y;
        }
    }

    private int TryFireFromTank(Tank tank, int cooldownCounter, int cooldownTicks)
    {
        if (!tank.IsAlive || cooldownCounter > 0)
        {
            return cooldownCounter;
        }

        var next = GetNextPosition(tank.X, tank.Y, tank.Direction);
        if (!Map.InBounds(next.x, next.y))
        {
            return cooldownTicks;
        }

        var bullet = new Bullet
        {
            X = next.x,
            Y = next.y,
            Direction = tank.Direction,
            FiredByPlayer = tank.IsPlayer
        };

        // Important fix: close-range shot checks impact immediately.
        if (!ResolveBulletImpact(bullet, bullet.X, bullet.Y))
        {
            Bullets.Add(bullet);
        }

        return cooldownTicks;
    }

    private void UpdateBullets()
    {
        for (var i = Bullets.Count - 1; i >= 0; i--)
        {
            var bullet = Bullets[i];
            var next = GetNextPosition(bullet.X, bullet.Y, bullet.Direction);
            if (!Map.InBounds(next.x, next.y))
            {
                Bullets.RemoveAt(i);
                continue;
            }

            if (ResolveBulletImpact(bullet, next.x, next.y))
            {
                Bullets.RemoveAt(i);
                continue;
            }

            bullet.X = next.x;
            bullet.Y = next.y;
        }

        HandleBulletVsBulletCollision();
    }

    private bool ResolveBulletImpact(Bullet bullet, int targetX, int targetY)
    {
        if (_collisionService.BulletShouldStop(Map, targetX, targetY))
        {
            var hitTile = Map.GetTile(targetX, targetY);
            if (_collisionService.BulletDestroysTile(Map, targetX, targetY))
            {
                Map.SetTile(targetX, targetY, TileType.Empty);
                if (hitTile == TileType.Base)
                {
                    IsGameOver = true;
                    StatusText = "Castle destroyed! Press R to restart.";
                }
            }

            AddExplosion(targetX, targetY);
            return true;
        }

        if (bullet.FiredByPlayer)
        {
            var enemy = Enemies.FirstOrDefault(t => t.IsAlive && t.X == targetX && t.Y == targetY);
            if (enemy is not null)
            {
                enemy.IsAlive = false;
                EnemyKills++;
                Score += 100;
                AddExplosion(enemy.X, enemy.Y);
                if (EnemyKills >= GameSettings.EnemyKillTarget)
                {
                    IsGameOver = true;
                    StatusText = "You win! Press R to restart.";
                }

                return true;
            }
        }
        else if (PlayerTank.IsAlive && PlayerTank.X == targetX && PlayerTank.Y == targetY)
        {
            AddExplosion(PlayerTank.X, PlayerTank.Y);
            HandlePlayerHit();
            return true;
        }

        return false;
    }

    private void HandlePlayerHit()
    {
        if (!PlayerTank.IsAlive || IsGameOver)
        {
            return;
        }

        PlayerLives--;
        if (PlayerLives <= 0)
        {
            PlayerTank.IsAlive = false;
            IsGameOver = true;
            StatusText = "Game over. Press R to restart.";
            return;
        }

        PlayerTank.X = 1;
        PlayerTank.Y = Map.Height - 2;
        PlayerTank.Direction = Direction.Up;
    }

    private bool IsOtherTankOnCell(Tank movingTank, int x, int y)
    {
        if (movingTank != PlayerTank && PlayerTank.IsAlive && PlayerTank.X == x && PlayerTank.Y == y)
        {
            return true;
        }

        foreach (var enemy in Enemies)
        {
            if (movingTank != enemy && enemy.IsAlive && enemy.X == x && enemy.Y == y)
            {
                return true;
            }
        }

        return false;
    }

    private void HandleBulletVsBulletCollision()
    {
        for (var i = Bullets.Count - 1; i >= 0; i--)
        {
            for (var j = i - 1; j >= 0; j--)
            {
                if (Bullets[i].X == Bullets[j].X && Bullets[i].Y == Bullets[j].Y)
                {
                    AddExplosion(Bullets[i].X, Bullets[i].Y);
                    Bullets.RemoveAt(i);
                    Bullets.RemoveAt(j);
                    break;
                }
            }
        }
    }

    private Direction SelectEnemyDirection(Tank enemy)
    {
        if (_random.Next(0, 100) < 45)
        {
            return (Direction)_random.Next(0, 4);
        }

        var baseX = Map.Width / 2;
        var baseY = Map.Height - 2;
        var targetX = _random.Next(0, 100) < 60 ? PlayerTank.X : baseX;
        var targetY = _random.Next(0, 100) < 60 ? PlayerTank.Y : baseY;
        var dx = targetX - enemy.X;
        var dy = targetY - enemy.Y;
        return Math.Abs(dx) > Math.Abs(dy)
            ? (dx < 0 ? Direction.Left : Direction.Right)
            : (dy < 0 ? Direction.Up : Direction.Down);
    }

    private void SpawnEnemiesIfNeeded()
    {
        while (Enemies.Count < GameSettings.MaxActiveEnemies && _enemiesSpawnedTotal < GameSettings.EnemyKillTarget)
        {
            var spawn = _enemySpawnPoints[_random.Next(0, _enemySpawnPoints.Length)];
            if (!_collisionService.CanMoveTo(Map, spawn.x, spawn.y) || IsOtherTankOnCell(PlayerTank, spawn.x, spawn.y))
            {
                break;
            }

            Enemies.Add(new Tank
            {
                IsPlayer = false,
                IsAlive = true,
                X = spawn.x,
                Y = spawn.y,
                Direction = Direction.Down,
                MoveCooldown = _random.Next(0, 2),
                ShootCooldown = _random.Next(1, 4)
            });
            _enemiesSpawnedTotal++;
        }
    }

    private void AddExplosion(int x, int y)
    {
        Explosions.Add(new Explosion
        {
            X = x,
            Y = y,
            RemainingTicks = GameSettings.ExplosionDurationTicks
        });
    }

    private void UpdateExplosions()
    {
        for (var i = Explosions.Count - 1; i >= 0; i--)
        {
            Explosions[i].RemainingTicks--;
            if (Explosions[i].RemainingTicks <= 0)
            {
                Explosions.RemoveAt(i);
            }
        }
    }

    private string BuildHudText()
    {
        var remaining = GameSettings.EnemyKillTarget - EnemyKills;
        return $"Lives: {PlayerLives} | Score: {Score} | Kills: {EnemyKills}/{GameSettings.EnemyKillTarget} | Enemies: {Enemies.Count} | Remaining: {remaining}";
    }

    private void UpdateHudText()
    {
        if (IsGameOver || IsPaused)
        {
            return;
        }

        StatusText = BuildHudText();
    }

    private void MapResetToDefault()
    {
        var newMap = Map.CreateDefault();
        for (var y = 0; y < Map.Height; y++)
        {
            for (var x = 0; x < Map.Width; x++)
            {
                Map.SetTile(x, y, newMap.GetTile(x, y));
            }
        }
    }

    private static (int x, int y) GetNextPosition(int x, int y, Direction direction)
    {
        return direction switch
        {
            Direction.Up => (x, y - 1),
            Direction.Down => (x, y + 1),
            Direction.Left => (x - 1, y),
            Direction.Right => (x + 1, y),
            _ => (x, y)
        };
    }
}
