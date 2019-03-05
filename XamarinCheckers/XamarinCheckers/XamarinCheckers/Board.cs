using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinCheckers
{
    class Board
    {
        private List<Piece> pieceList;
        private List<Move> moveList;

        public Board()
        {
            this.pieceList = new List<Piece>();
        }

        public List<Move> findMoves()
        {
            return new List<Move>();
        }

        public Piece findPiece(Location loc)
        {
            foreach (Piece piece in pieceList)
            {
                if (piece.location == loc)
                    return piece;
            }
            return null;
        }

        public List<Move> findMovesForPiece(Piece piece)
        {
            return null;
        }

        public List<Move> findMoves(Color color)
        {
            return null;
        }

        public List<Move> findCapturingMoves(Piece piece)
        {
            return null;
        }

        public bool validate(Move move)
        {
            return false;
        }

        public void applyMove(Move move)
        {

        }

        public void removePiece(Piece piece)
        {

        }

        public bool isInWinState()
        {
            return false;
        }


    }
}
