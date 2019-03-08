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

        public MainPage()
        {
            InitializeComponent();
            gameBoard = new Board();
            gameBoard.NewCheckersGame();
            moveRecs = new List<Move>();
            turn = (Color)0;
        }

        private void ClickedGrid(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Location square = new Location(Grid.GetColumn(btn), Grid.GetRow(btn));
            Console.WriteLine("Clicked on " + Grid.GetColumn(btn) + ", " + Grid.GetRow(btn));
            Piece p = gameBoard.FindPiece(turn, square);
            if (p != null)
            {
                Console.WriteLine("Found Piece");
                foreach (Move move in moveRecs)
                {
                    Button empty = new Button { };
                    empty.Clicked += ClickedGrid;
                    boardGrid.Children.Add(empty, move.endLoc.xCoord, move.endLoc.yCoord);
                }
                moveRecs = gameBoard.FindMovesForPiece(p);
                foreach (Move m in moveRecs)
                {
                    Location l = m.endLoc;
                    Console.WriteLine("Trying to highlight possible move at " + l.xCoord + "," + l.yCoord);
                    Button highlight = new Button { BackgroundColor = Xamarin.Forms.Color.Yellow };
                    highlight.Clicked += ClickedGrid;
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
                        Button empty = new Button {};
                        empty.Clicked += ClickedGrid;
                        Button movedChecker = new Button { Text = "C" };
                        movedChecker.Clicked += ClickedGrid;
                        boardGrid.Children.Add(empty, m.movingPiece.location.xCoord, m.movingPiece.location.yCoord);
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
                    Button empty = new Button { };
                    empty.Clicked += ClickedGrid;
                    if (gameBoard.FindPiece(move.endLoc) == null)
                        boardGrid.Children.Add(empty, move.endLoc.xCoord, move.endLoc.yCoord);
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
