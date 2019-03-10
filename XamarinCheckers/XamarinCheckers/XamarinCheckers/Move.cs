using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinCheckers
{
    public class Move
    {
        public Piece movingPiece;
        public Location endLoc;
        public List<Piece> capturedPieces;
        public bool undo;
        public bool forfeit;

        public Move(Piece piece, Location end)
        {
            movingPiece = piece;
            endLoc = end;
            capturedPieces = new List<Piece>();
        }

        public Move(bool forfeit)
        {
            forfeit = true;
        }

        public Move()
        {
        }
        

    }
}
