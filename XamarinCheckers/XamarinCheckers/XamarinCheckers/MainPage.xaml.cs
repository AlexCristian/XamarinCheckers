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
        private double timeout;
        private Color turn;
        private Board gameBoard;
        private ImageButton redChecker = new ImageButton { Source = "redchecker.jpg" };
        private ImageButton blackChecker = new ImageButton { Source = "graychecker.jpg" };

        public MainPage()
        {
            InitializeComponent();
            gameBoard = new Board();
            gameBoard.NewCheckersGame();
            moveRecs = new List<Move>();
            turn = (Color)0;
            redChecker.Clicked += ClickedGrid;
            blackChecker.Clicked += ClickedGrid;
            System.Diagnostics.Debug.WriteLine("My ip is: " + Network.GetDeviceIPAddress());
            Connection con = Network.ListenForOpponent().Result;
            Move move = new Move(new Piece(Color.Red, new Location(3, 5)), new Location(3,6));
            con.SendMove(move).Wait();
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
                moveRecs = gameBoard.FindMovesForPiece(p);
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
                        if (turn == (Color)0)
                            movedChecker = blackChecker;
                        else
                            movedChecker = redChecker;
                        ImageButton emptyBoard = new ImageButton { Source = "blackboard.jpg" };
                        emptyBoard.Clicked += ClickedGrid;
                        boardGrid.Children.Add(emptyBoard, m.movingPiece.location.xCoord, m.movingPiece.location.yCoord);
                        boardGrid.Children.Add(movedChecker, m.endLoc.xCoord, m.endLoc.yCoord);
                        gameBoard.ApplyMove(m);
                        if (gameBoard.IsInWinState())
                        {
                            Color winner = gameBoard.GetWinner();
                            boardGrid.Children.Add(new Label { Text = "WINNER" }, 0, 0);
                        }
                        else
                            SwapTurn();
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
                turn = (Color)1;
            else
                turn = (Color)0;
        }
    }
}
