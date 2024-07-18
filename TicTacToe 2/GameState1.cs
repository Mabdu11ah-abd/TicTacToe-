using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe_2
{
    internal class GameState
    {
        public Player[,] GameGrid { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public int TurnsPassed { get; private set; }
        public bool GameOver { get; private set; }

        public event Action<int, int> MoveMade;
        public event Action<GameResult> GameEnded;
        public event Action GameRestarted;

        public GameState()
        {
            GameGrid = new Player[3, 3];
            CurrentPlayer = Player.X;
            TurnsPassed = 0;
            GameOver = false;
        }
        private bool canMakeMove(int r, int c)
        {
            return !GameOver && !(GameGrid[r, c] == Player.none);
        }
        private bool isGridFull()
        {
            return TurnsPassed == 9;
        }
        private void switchPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player.X ? Player.O : Player.X;
        }
        private bool areSquaresMarked((int, int)[] squares, Player player)
        {
            foreach ((int r, int c) in squares)
            {
                if (GameGrid[r, c] != player)
                {
                    Debug.WriteLine("Squares were marked");
                    return false;
                }
            }

            return true;
        }
        private bool DidMoveWin(int r, int c, out WinInfo winInfo)
        {
            (int, int)[] row = new[] { (r, 0), (r, 1), (r, 2) };
            (int, int)[] col = new[] { (0, c), (1, c), (2, c) };
            (int, int)[] diag1 = new[] { (0, 0), (1, 1), (2, 2) };
            (int, int)[] diag2 = new[] { (0, 2), (1, 1), (2, 0) };

            if (areSquaresMarked(row, CurrentPlayer))
            {
                winInfo = new WinInfo { number = r, Type = WinType.horizontal };
                return true;
            }
            if (areSquaresMarked(col, CurrentPlayer))
            {
                winInfo = new WinInfo { number = c, Type = WinType.vertical };
                return true;
            }
            if (areSquaresMarked(diag1, CurrentPlayer))
            {
                winInfo = new WinInfo { Type = WinType.diagonal1 };
                return true;
            }
            if (areSquaresMarked(diag2, CurrentPlayer))
            {
                Debug.WriteLine("Diagonal 2 won wininfo object passed");
                winInfo = new WinInfo { Type = WinType.diagonal2 };
                return true;
            }
            winInfo = null;
            return false;
        }

        private bool didMoveEndGame(int r, int c, out GameResult gameresult)
        {
            if (DidMoveWin(r, c, out WinInfo wininfo))
            {
                gameresult = new GameResult { info = wininfo, player = CurrentPlayer };
                return true;
            }
            if (isGridFull())
            {
                gameresult = new GameResult { player = Player.none };
                return true;
            }
            gameresult = null;
            return false;
        }
        public void makeMove(int r, int c)
        {
            //checks
            if (canMakeMove(r, c))
            {
                return;
            }
            // simple move
            GameGrid[r, c] = CurrentPlayer;
            TurnsPassed++;
            //move that endsgame
            if (didMoveEndGame(r, c, out GameResult gameresult))
            {

                GameOver = true;
                MoveMade?.Invoke(r, c);
                GameEnded?.Invoke(gameresult);
            }
            else
            {
                switchPlayer();
                MoveMade?.Invoke(r, c);
            }
        }
        public void GameRestart()
        {
            GameGrid = new Player[3, 3];
            CurrentPlayer = Player.X;
            TurnsPassed = 0;
            GameOver = false;
            GameRestarted?.Invoke();
        }
    }

}
