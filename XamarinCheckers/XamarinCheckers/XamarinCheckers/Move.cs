using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinCheckers
{
    class Move
    {
        public Piece movingPiece;
        public Location endLoc;
        public List<Piece> capturedPieces;
        public bool undo;

        public Move(Piece piece, Location end)
        {
            movingPiece = piece;
            endLoc = end;
            capturedPieces = new List<Piece>();
        }

        public Move(Piece piece, Location end, List<Piece> capt)
        {
            movingPiece = piece;
            endLoc = end;
            capturedPieces = capt;
        }

    }
}
