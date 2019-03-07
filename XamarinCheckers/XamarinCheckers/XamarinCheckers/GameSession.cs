using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinCheckers
{
    class GameSession
    {
        private double timeout;
        private Color turn;
        private Board gameBoard;

        public GameSession()
        {
            turn = (Color)0;
            gameBoard = new Board();
            gameBoard.newCheckersGame();
        }

        public List<Location> processSelection(Location l)
        {
            Piece p = gameBoard.findPiece(turn, l);
            List<Location> highlights = new List<Location>();
            if (p != null)
            {
                Console.WriteLine("Found Piece");
                foreach (Move m in gameBoard.findMovesForPiece(p))
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
            gameBoard.applyMove(m);
        }

        private void terminate()
        {

        }

    }
}
