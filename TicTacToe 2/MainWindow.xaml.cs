using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TicTacToe_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<Player, ImageSource> imageSources = new()
        {
            { Player.X, new BitmapImage(new Uri("Assets/Cross.png", UriKind.Relative))},
            { Player.O, new BitmapImage(new Uri("Assets/Circle.png", UriKind.Relative))}
        };
        private readonly Image[,] imageControls = new Image[3, 3];
        private readonly GameState gamestate = new GameState();
        public MainWindow()
        {
            InitializeComponent();
            setup_GameGrid();

            gamestate.MoveMade += onMoveMade;
            gamestate.GameEnded += onGameEnd;
            gamestate.GameRestarted += onGameRestarted;
        }
        private void setup_GameGrid()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Image imageControl = new Image();
                    GameGrid.Children.Add(imageControl);
                    imageControls[i, j] = imageControl;
                }
            }
        }
        private void onMoveMade(int r, int c)
        {
            Player currentPlayer = gamestate.GameGrid[r, c];
            Debug.WriteLine("Player image has been changed");
            imageControls[r, c].Source = imageSources[currentPlayer];
        }
        private void onGameRestarted()
        {
            for (int i = 0; i < 3; i++)
            {
                for (global::System.Int32 j = 0; j < 3; j++)
                {
                    imageControls[i, j].Source = null;
                }
            }
            WinLine.Visibility = Visibility.Hidden;
            transitiontoGameScreen();
        }
        private async void onGameEnd(GameResult gameResult)
        {

            Debug.WriteLine("Method Called");
            if (gameResult.player == Player.none)
            {
                Debug.WriteLine("No one won");
                transitionToEndScreen();
                
            }
            else
            {
                ToggleLineVisibility(gameResult.info);
                await Task.Delay(1000);
                transitionToEndScreen();
            }
        }
        private (Point, Point) findCoords(WinInfo winInfo)
        {
            double squareSize = GameGrid.Width / 3;
            double margin = squareSize / 2;

            if (winInfo.Type == WinType.horizontal)
            {
                double y = winInfo.number * squareSize + margin;
                return (new Point(0, y), new Point(GameGrid.Width, y));
            }
            if (winInfo.Type == WinType.vertical)
            {
                double x = winInfo.number * squareSize + margin;
                return (new Point(x, 0), new Point(x, GameGrid.Width));
            }
            if (winInfo.Type == WinType.diagonal1)
            {
                return (new Point(0, 0), new Point(GameGrid.Width, GameGrid.Height));
            }
            return (new Point(GameGrid.Width, 0), new Point(0, GameGrid.Height));
        }
        private void ToggleLineVisibility(WinInfo winInfo)
        {
            (Point, Point) mouseCoords = findCoords(winInfo);
            WinLine.X1 = mouseCoords.Item1.X;
            WinLine.Y1 = mouseCoords.Item1.Y;

            WinLine.X2 = mouseCoords.Item2.X;
            WinLine.Y2 = mouseCoords.Item2.Y;

            WinLine.Visibility = Visibility.Visible;
        }
    
    private void GameGrid_MouseDown(object sender, MouseEventArgs e)
        {
            double squareSize = GameGrid.Width / 3;
            Point clickPosition = e.GetPosition(GameGrid);
            int row = (int)(clickPosition.Y / squareSize);
            int col = (int)(clickPosition.X / squareSize);
            Debug.WriteLine(row + " " + col);
            gamestate.makeMove(row, col);
        }
        private void transitionToEndScreen()
        {
            EndScreen.Visibility = Visibility.Visible;
        }
        private void transitiontoGameScreen()
        {
            EndScreen.Visibility = Visibility.Hidden;
        }
        private void RestartBtn_Click(object sender, RoutedEventArgs e)
        {
            gamestate.GameRestart();
        }
    }
}