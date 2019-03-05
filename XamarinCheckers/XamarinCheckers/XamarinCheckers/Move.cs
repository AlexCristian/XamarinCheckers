using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinCheckers
{
    class Move
    {
        private Piece movingPiece;
        private Location endLoc;
        private List<Piece> capturedPieces;
        private bool undo;

        public Move(Piece piece, Location end)
        {
            movingPiece = piece;
            endLoc = end;
            capturedPieces = new List<Piece>();
        }

    }
}
