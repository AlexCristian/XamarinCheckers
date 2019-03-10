using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinCheckers
{
    public partial class MainPage : ContentPage
    {
        private List<Move> moveRecs;
        private List<Piece> highlightPieces;
        private double timeout;
        private Color turn;
        private Board gameBoard;

        public MainPage()
        {
            InitializeComponent();
            gameBoard = new Board();
            gameBoard.NewCheckersGame();
            moveRecs = new List<Move>();
            highlightPieces = new List<Piece>();
            turn = (Color)0;
            System.Diagnostics.Debug.WriteLine("My ip is: " + Network.GetDeviceIPAddress());
            /* Server - Network demo code
            Connection con = Network.ListenForOpponent().Result;
            Move move = new Move(new Piece(Color.Red, new Location(3, 5)), new Location(3,6));
            con.SendMove(move).Wait();*/

            /* Receiver - Network demo code
            Connection con = Network.ConnectWithOpponent("192.168.86.110").Result;
            Move move = con.ListenForMove().Result;
            move.forfeit = false;*/
        }

        private void ClickedGrid(object sender, EventArgs e)
        {
            ImageButton btn = sender as ImageButton;
            Location square = new Location(Grid.GetColumn(btn), Grid.GetRow(btn));
            Console.WriteLine("Clicked on " + Grid.GetColumn(btn) + ", " + Grid.GetRow(btn));
            Piece p = gameBoard.FindPiece(turn, square);
            if (p != null)
            {
                Console.WriteLine("Found Piece");
                foreach (Move move in moveRecs)
                {
                    ImageButton emptyBoard = new ImageButton { Source = "blackboard.jpg" };
                    emptyBoard.Clicked += ClickedGrid;
                    boardGrid.Children.Add(emptyBoard, move.endLoc.xCoord, move.endLoc.yCoord);
                }
                if (highlightPieces.Count == 0 || highlightPieces.Contains(p))
                    moveRecs = gameBoard.FindMovesForPiece(p);
                else
                    moveRecs = new List<Move>();
                foreach (Move m in moveRecs)
                {
                    ImageButton highlight = new ImageButton { Source = "highlightboard.jpg" };
                    highlight.Clicked += ClickedGrid;
                    Location l = m.endLoc;
                    Console.WriteLine("Trying to highlight possible move at " + l.xCoord + "," + l.yCoord); 
                    boardGrid.Children.Add(highlight, l.xCoord, l.yCoord);
                }
            }
            else
            {
                Console.WriteLine("No piece of your color there");
                foreach (Move m in moveRecs)
                {
                    if (m.endLoc == square && gameBoard.Validate(m))
                    {
                        ImageButton movedChecker;
                        if (gameBoard.IsKingSpace(m.endLoc, turn) || m.movingPiece.rank == Rank.King)
                        {
                            m.movingPiece.rank = Rank.King;
                            if (turn == (Color)0)
                                movedChecker = new ImageButton { Source = "graycheckerking.jpg" };
                            else
                                movedChecker = new ImageButton { Source = "redcheckerking.jpg" };
                        }
                        else if (turn == (Color)0)
                            movedChecker = new ImageButton { Source = "graychecker.jpg" };
                        else
                            movedChecker = new ImageButton { Source = "redchecker.jpg" };
                        movedChecker.Clicked += ClickedGrid;
                        ImageButton emptyBoard = new ImageButton { Source = "blackboard.jpg" };
                        emptyBoard.Clicked += ClickedGrid;
                        boardGrid.Children.Add(emptyBoard, m.movingPiece.location.xCoord, m.movingPiece.location.yCoord);
                        boardGrid.Children.Add(movedChecker, m.endLoc.xCoord, m.endLoc.yCoord);
                        foreach (Piece capt in m.capturedPieces)
                        {
                            ImageButton anotherEmptyBoard = new ImageButton { Source = "blackboard.jpg" };
                            boardGrid.Children.Add(anotherEmptyBoard, capt.location.xCoord, capt.location.yCoord);
                        }
                        gameBoard.ApplyMove(m);
                        foreach (Piece light in highlightPieces)
                        {
                            ImageButton normChecker;
                            if (light.rank == Rank.King)
                            {
                                if (turn == (Color)0)
                                    normChecker = new ImageButton { Source = "graycheckerking.jpg" };
                                else
                                    normChecker = new ImageButton { Source = "redcheckerking.jpg" };
                            }
                            else if (turn == (Color)0)
                                normChecker = new ImageButton { Source = "graychecker.jpg" };
                            else
                                normChecker = new ImageButton { Source = "redchecker.jpg" };
                            normChecker.Clicked += ClickedGrid;
                            boardGrid.Children.Add(normChecker, light.location.xCoord, light.location.yCoord);
                        }
                        highlightPieces.Clear();
                        if (gameBoard.IsInWinState())
                        {
                            Color winner = gameBoard.GetWinner();
                            if (winner == (Color)0)
                                turnTracker.Text = "WINNER! BLACK";
                            else
                                turnTracker.Text = "WINNER! RED";
                        }
                        else
                        {
                            SwapTurn();
                            List<Move> captMoves = gameBoard.FindCapturingMoves(turn);
                            if (captMoves.Count > 0)
                            {
                                foreach (Move captMove in captMoves)
                                {
                                    Piece capturer = captMove.movingPiece;
                                    highlightPieces.Add(capturer);
                                    ImageButton lightChecker;
                                    if (capturer.rank == Rank.King)
                                    {
                                        if (turn == (Color)0)
                                            lightChecker = new ImageButton { Source = "graycheckerkinglight.jpg" };
                                        else
                                            lightChecker = new ImageButton { Source = "redcheckerkinglight.jpg" };
                                    }
                                    else if (turn == (Color)0)
                                        lightChecker = new ImageButton { Source = "graycheckerlight.jpg" };
                                    else
                                        lightChecker = new ImageButton { Source = "redcheckerlight.jpg" };
                                    lightChecker.Clicked += ClickedGrid;
                                    boardGrid.Children.Add(lightChecker, capturer.location.xCoord, capturer.location.yCoord);
                                }
                            }
                        }
                        moveRecs.Remove(m);
                        break;
                    }
                }
                foreach (Move move in moveRecs)
                {
                    ImageButton emptyBoard = new ImageButton { Source = "blackboard.jpg" };
                    emptyBoard.Clicked += ClickedGrid;
                    if (gameBoard.FindPiece(move.endLoc) == null)
                        boardGrid.Children.Add(emptyBoard, move.endLoc.xCoord, move.endLoc.yCoord);
                }
                moveRecs.Clear();
            }
        }

        private void SwapTurn()
        {
            if (turn == (Color)0)
            {
                turnTracker.Text = "Red's Turn";
                turn = (Color)1;
            }
            else
            {
                turn = (Color)0;
                turnTracker.Text = "Black's Turn";
            }
        }
    }
}
