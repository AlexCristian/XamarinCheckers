using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinCheckers
{
    class Board
    {
        private List<Piece> playerOnePieces;
        private List<Piece> playerTwoPieces;

        public Board()
        {
            playerOnePieces = new List<Piece>();
            playerTwoPieces = new List<Piece>();
        }

        // set up a checkerboard for a new game
        public void NewCheckersGame()
        {
            playerTwoPieces = new List<Piece>();
            playerTwoPieces.Add(new Piece(Color.Red, new Location(0, 0)));
            playerTwoPieces.Add(new Piece(Color.Red, new Location(2, 0)));
            playerTwoPieces.Add(new Piece(Color.Red, new Location(4, 0)));
            playerTwoPieces.Add(new Piece(Color.Red, new Location(6, 0)));

            playerTwoPieces.Add(new Piece(Color.Red, new Location(1, 1)));
            playerTwoPieces.Add(new Piece(Color.Red, new Location(3, 1)));
            playerTwoPieces.Add(new Piece(Color.Red, new Location(5, 1)));
            playerTwoPieces.Add(new Piece(Color.Red, new Location(7, 1)));

            playerTwoPieces.Add(new Piece(Color.Red, new Location(0, 2)));
            playerTwoPieces.Add(new Piece(Color.Red, new Location(2, 2)));
            playerTwoPieces.Add(new Piece(Color.Red, new Location(4, 2)));
            playerTwoPieces.Add(new Piece(Color.Red, new Location(6, 2)));

            playerOnePieces = new List<Piece>();
            playerOnePieces.Add(new Piece(Color.Black, new Location(1, 5)));
            playerOnePieces.Add(new Piece(Color.Black, new Location(3, 5)));
            playerOnePieces.Add(new Piece(Color.Black, new Location(5, 5)));
            playerOnePieces.Add(new Piece(Color.Black, new Location(7, 5)));

            playerOnePieces.Add(new Piece(Color.Black, new Location(0, 6)));
            playerOnePieces.Add(new Piece(Color.Black, new Location(2, 6)));
            playerOnePieces.Add(new Piece(Color.Black, new Location(4, 6)));
            playerOnePieces.Add(new Piece(Color.Black, new Location(6, 6)));

            playerOnePieces.Add(new Piece(Color.Black, new Location(1, 7)));
            playerOnePieces.Add(new Piece(Color.Black, new Location(3, 7)));
            playerOnePieces.Add(new Piece(Color.Black, new Location(5, 7)));
            playerOnePieces.Add(new Piece(Color.Black, new Location(7, 7)));
        }

        // checks if a location counts for kinging
        public bool IsKingSpace(Location loc, Color turn)
        {
            List<Location> lastRow;
            if (turn == (Color)0)
                lastRow = new List<Location> { new Location(0, 0), new Location(2, 0), new Location(4, 0), new Location(6, 0) };
            else
                lastRow = new List<Location> { new Location(1, 7), new Location(3, 7), new Location(5, 7), new Location(7, 7) };
            foreach (Location l in lastRow)
                if (l == loc)
                    return true;
            return false;
        }

        // locate a piece, or return null if one is not there
        public Piece FindPiece(Location loc)
        {
            foreach (Piece piece in playerOnePieces)
            {
                if (piece.location == loc)
                    return piece;
            }
            foreach (Piece piece in playerTwoPieces)
            {
                if (piece.location == loc)
                    return piece;
            }
            return null;
        }

        // find piece for which expected color is known
        public Piece FindPiece(Color c, Location l)
        {
            List<Piece> searchList;
            if (c == (Color)0)
                searchList = playerOnePieces;
            else
                searchList = playerTwoPieces;
            foreach (Piece piece in searchList)
            {
                if (piece.location == l)
                    return piece;
            }
            return null;
        }

        // find all moves for one player
        // calls findMovesForPiece
        public List<Move> FindMoves(Color color)
        {
            List<Piece> pieceList;
            if ((int)color == 0)
                pieceList = playerOnePieces;
            else
                pieceList = playerTwoPieces;
            List<Move> colorMoves = new List<Move>();
            foreach (Piece piece in pieceList)
                colorMoves.AddRange(FindMovesForPiece(piece));
            return colorMoves;
        }

        // similar to above, but removes all non-capturing moves
        public List<Move> FindCapturingMoves(Color color)
        {
            List<Piece> pieceList;
            if ((int)color == 0)
                pieceList = playerOnePieces;
            else
                pieceList = playerTwoPieces;
            List<Move> colorMoves = new List<Move>();
            foreach (Piece piece in pieceList)
                colorMoves.AddRange(FindMovesForPiece(piece));
            List<Move> captureMoves = new List<Move>();
            foreach (Move m in colorMoves)
            {
                if (m.capturedPieces.Count > 0)
                    captureMoves.Add(m);
            }
            return captureMoves;
        }


        // returns a list of possible moves for a given piece
        // if a capturing move is available one HAS to take it
        public List<Move> FindMovesForPiece(Piece piece)
        {
            List<Move> moveList = new List<Move>();
            List<Location> destinations = new List<Location>();
            List<Location> jumpDestinations = new List<Location>();
            if (piece.rank == Rank.King || piece.color == (Color)1)
            {
                destinations.Add(new Location(piece.location.xCoord + 1, piece.location.yCoord + 1));
                destinations.Add(new Location(piece.location.xCoord - 1, piece.location.yCoord + 1));
                jumpDestinations.Add(new Location(piece.location.xCoord + 2, piece.location.yCoord + 2));
                jumpDestinations.Add(new Location(piece.location.xCoord - 2, piece.location.yCoord + 2));
            }
            if (piece.rank == Rank.King || piece.color == (Color)0)
            {
                destinations.Add(new Location(piece.location.xCoord + 1, piece.location.yCoord - 1));
                destinations.Add(new Location(piece.location.xCoord - 1, piece.location.yCoord - 1));
                jumpDestinations.Add(new Location(piece.location.xCoord + 2, piece.location.yCoord - 2));
                jumpDestinations.Add(new Location(piece.location.xCoord - 2, piece.location.yCoord - 2));
            }

            foreach (Location loc in jumpDestinations)
            {
                Location jumpedLoc = new Location((piece.location.xCoord + loc.xCoord) / 2, (piece.location.yCoord + loc.yCoord) / 2);
                // verify that jumped piece is opposite color
                if (NotOffBoard(loc) && FindPiece(loc) == null 
                    && FindPiece(jumpedLoc) != null 
                    && FindPiece(jumpedLoc).color != piece.color)

                {
                    Move jumpMove = new Move(piece, loc, new List<Piece> { FindPiece(jumpedLoc) });
                    moveList.Add(CheckJumpMove(jumpMove, piece.location));
                }
            }
            if (moveList.Count != 0)
                return moveList;
            else
            {
                foreach (Location loc in destinations)
                {
                    if (NotOffBoard(loc) && FindPiece(loc) == null)
                    {
                        moveList.Add(new Move(piece, loc));
                    }
                }
                return moveList;
            }
        }

        // recursively check if another jump can be made from this location
        private Move CheckJumpMove(Move move, Location priorDest)
        {
            Location dest = move.endLoc;
            List<Location> nextJumpDest = new List<Location>();
            if (move.movingPiece.rank == Rank.King || move.movingPiece.color == (Color)1)
            {
                nextJumpDest.Add(new Location(dest.xCoord + 2, dest.yCoord + 2));
                nextJumpDest.Add(new Location(dest.xCoord - 2, dest.yCoord + 2));
            }
            if (move.movingPiece.rank == Rank.King || move.movingPiece.color == (Color)0)
            {
                nextJumpDest.Add(new Location(dest.xCoord + 2, dest.yCoord - 2));
                nextJumpDest.Add(new Location(dest.xCoord - 2, dest.yCoord - 2));
            }
            foreach (Location l in nextJumpDest)
            {
                if (l == priorDest)
                {
                    nextJumpDest.Remove(l);
                    break;
                }
            }
            foreach (Location loc in nextJumpDest)
            {
                Location jumpedLoc = new Location((dest.xCoord + loc.xCoord) / 2, (dest.yCoord + loc.yCoord) / 2);
                if (NotOffBoard(loc) && FindPiece(loc) == null
                    && FindPiece(jumpedLoc) != null
                    && FindPiece(jumpedLoc).color != move.movingPiece.color)

                {
                    Console.WriteLine("Trying to find recursive move");
                    move.capturedPieces.Add(FindPiece(jumpedLoc));
                    move.endLoc = loc;
                    return CheckJumpMove(move, dest);
                }
            }
            return move;
        }

        // just a helper method to check if a location is allowed
        private bool NotOffBoard(Location loc)
        {
            if (loc.xCoord >= 0 && loc.xCoord <= 7 && loc.yCoord >= 0 && loc.yCoord <= 7)
                return true;
            else
                return false;
        }

        // validate move???
        // TODO
        public bool Validate(Move move)
        {
            return true;
        }

        // finalize a move
        // updates piece location and removes captured pieces
        public void ApplyMove(Move move)
        {
            Piece movedPiece = FindPiece(move.movingPiece.color, move.movingPiece.location);
            movedPiece.location = move.endLoc;
            foreach (Piece p in move.capturedPieces)
                RemovePiece(p);
        }

        // take a piece out of play
        // TODO: also need to implement captured pieces counter?
        public void RemovePiece(Piece piece)
        {
            Piece capturedPiece = FindPiece(piece.color, piece.location);
            if (capturedPiece.color == (Color)0)
                playerOnePieces.Remove(capturedPiece);
            else
                playerTwoPieces.Remove(capturedPiece);
        }

        // check whether the game should be over
        public bool IsInWinState()
        {
            if (playerOnePieces.Count == 0 || FindMoves((Color)1).Count == 0 || playerTwoPieces.Count == 0 || FindMoves((Color)0).Count == 0)
                return true;
            return false;
        }
        
        // declare a winner - should only be called if isInWinState == true
        public Color GetWinner()
        {
            if (playerOnePieces.Count == 0 || FindMoves((Color)1).Count == 0)
                return (Color)0;
            else
                return (Color)1;
        }


    }
}
