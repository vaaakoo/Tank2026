using System.Windows;
using System.Windows.Input;
using Tank2026.Core;
using Tank2026.Services;
using Tank2026.UI;

namespace Tank2026
{
    public partial class MainWindow : Window
    {
        private readonly GameEngine _gameEngine;
        private readonly GameRenderer _renderer;

        public MainWindow()
        {
            InitializeComponent();

            GameCanvas.Width = GameSettings.MapWidth * GameSettings.TileSize;
            GameCanvas.Height = GameSettings.MapHeight * GameSettings.TileSize;

            _gameEngine = new GameEngine(new CollisionService());
            _renderer = new GameRenderer(GameCanvas);

            _gameEngine.GameUpdated += OnGameUpdated;
            _renderer.Render(_gameEngine);
            StatusTextBlock.Text = _gameEngine.StatusText;
        }

        protected override void OnClosed(EventArgs e)
        {
            _gameEngine.Stop();
            base.OnClosed(e);
        }

        private void OnGameUpdated()
        {
            _renderer.Render(_gameEngine);
            StatusTextBlock.Text = _gameEngine.StatusText;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (MenuOverlay.Visibility == Visibility.Visible)
                    {
                        MenuOverlay.Visibility = Visibility.Collapsed;
                        _gameEngine.Start();
                    }
                    break;
                case Key.Up:
                    _gameEngine.MovePlayer(Direction.Up);
                    break;
                case Key.Down:
                    _gameEngine.MovePlayer(Direction.Down);
                    break;
                case Key.Left:
                    _gameEngine.MovePlayer(Direction.Left);
                    break;
                case Key.Right:
                    _gameEngine.MovePlayer(Direction.Right);
                    break;
                case Key.Space:
                    _gameEngine.PlayerShoot();
                    break;
                case Key.R:
                    _gameEngine.Restart();
                    break;
                case Key.P:
                    _gameEngine.TogglePause();
                    break;
                default:
                    return;
            }

            _renderer.Render(_gameEngine);
            e.Handled = true;
        }
    }
}