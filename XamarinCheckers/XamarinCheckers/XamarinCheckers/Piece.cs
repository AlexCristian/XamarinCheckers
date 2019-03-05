using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinCheckers
{
    enum Color
    {
        Black,
        Red
    }

    enum Rank
    {
        Checker,
        King
    }

    class Piece
    {
        public Location location;
        public Rank rank;
        public Color color;


        private Piece(Color color, Location location)
        {
            this.color = color;
            this.location = location;
            this.rank = Rank.Checker;
        }

        private void kingPiece()
        {
            this.rank = Rank.King;
        }

        public List<Move> getAllowedMoves(Board board)
        {
            return null;
        }
    }
}
