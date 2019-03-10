using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinCheckers
{
    public class Move
    {
        public Location startLoc;
        public Rank pieceRank;
        public Color pieceColor;
        public Location endLoc;
        public List<Location> capturedPieceLocs;
        public bool undo;
        public bool forfeit;

        public Move()
        {
        }

        public Move(bool forfeit)
        {
            forfeit = true;
        }

        public Move(Piece piece, Location end)
        {
            startLoc = piece.location;
            pieceColor = piece.color;
            pieceRank = piece.rank;
            endLoc = end;
            capturedPieceLocs = new List<Location>();
        }
        
        public Move(Piece piece, Location end, List<Location> capt)
        {
            startLoc = piece.location;
            pieceColor = piece.color;
            pieceRank = piece.rank;
            endLoc = end;
            capturedPieceLocs = capt;
        }
    }
}
