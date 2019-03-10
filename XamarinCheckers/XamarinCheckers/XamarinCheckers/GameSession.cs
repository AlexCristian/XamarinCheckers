using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinCheckers
{
    class GameSession
    {
        private double timeout;
        public Color turn;
        public Board gameBoard;

        public GameSession()
        {
            turn = (Color)0;
            gameBoard = new Board();
            gameBoard.NewCheckersGame();
        }

        public List<Location> processSelection(Location l)
        {
            Piece p = gameBoard.FindPiece(turn, l);
            List<Location> highlights = new List<Location>();
            if (p != null)
            {
                Console.WriteLine("Found Piece");
                foreach (Move m in gameBoard.FindMovesForPiece(p))
                    highlights.Add(m.endLoc);
            }
            else
                Console.WriteLine("Your piece isn't there");
            return highlights;
        }

        public void swapTurn()
        {
            if (turn == (Color)0)
                turn = (Color)1;
            else
                turn = (Color)0;
        }

        private void makeMove(Move m)
        {
            gameBoard.ApplyMove(m);
        }

    }
}
