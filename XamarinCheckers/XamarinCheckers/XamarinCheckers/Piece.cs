using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinCheckers
{
    public enum Color
    {
        Black,
        Red
    }

    public enum Rank
    {
        Checker,
        King
    }

    public class Piece
    {
        public Location location;
        public Rank rank;
        public Color color;


        public Piece(Color color, Location location)
        {
            this.color = color;
            this.location = location;
            this.rank = Rank.Checker;
        }

        public Piece()
        {
        }

        private void kingPiece()
        {
            this.rank = Rank.King;
        }

    }
}
