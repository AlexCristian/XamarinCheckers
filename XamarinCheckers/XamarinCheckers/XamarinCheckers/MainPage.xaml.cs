using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinCheckers
{

    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
                return null;
            
            var imageSource = ImageSource.FromResource(Source);

            return imageSource;
        }
    }

    public partial class MainPage : ContentPage
    {
        private List<Move> moveRecs;
        private List<Piece> highlightPieces;
        private double timeout;
        private Color turn, localColor;
        private Board gameBoard;
        private Connection opponent;

        public MainPage()
        {
            InitializeComponent();
            gameBoard = new Board();
            gameBoard.NewCheckersGame();
            moveRecs = new List<Move>();
            highlightPieces = new List<Piece>();
            turn = (Color)0;
            System.Diagnostics.Debug.WriteLine("My ip is: " + Network.GetDeviceIPAddress());
            /*
            opponent = Network.ListenForOpponent().Result;
            localColor = (Color)0;*/

            /* Receiver - Network demo code*/
            opponent = Network.ConnectWithOpponent("192.168.86.110").Result;
            localColor = (Color)1;
            ListenForRemoteMove();
        }

        private async void ClickedGrid(object sender, EventArgs e)
        {
            ImageButton btn = sender as ImageButton;
            Location square = new Location(Grid.GetColumn(btn), Grid.GetRow(btn));
            Console.WriteLine("Clicked on " + Grid.GetColumn(btn) + ", " + Grid.GetRow(btn));
            Piece p = gameBoard.FindPiece(turn, square);

            if (turn != localColor)
            {
                return;
            }

            if (p != null)
            {
                Console.WriteLine("Found Piece");
                foreach (Move move in moveRecs)
                {
                    ImageButton emptyBoard = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.blackboard.jpg") };
                    emptyBoard.Clicked += ClickedGrid;
                    boardGrid.Children.Add(emptyBoard, move.endLoc.xCoord, move.endLoc.yCoord);
                }
                if (highlightPieces.Count == 0 || highlightPieces.Contains(p))
                    moveRecs = gameBoard.FindMovesForPiece(p);
                else
                    moveRecs = new List<Move>();
                foreach (Move m in moveRecs)
                {
                    ImageButton highlight = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.highlightboard.jpg") };
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
                        ApplyMoveToUI(m);

                        turnTracker.Text = "Sending move...";
                        await opponent.SendMove(m);

                        SwapTurn();
                        if (!gameBoard.IsInWinState())
                        {
                            if (turn == localColor)
                            {
                                ForceCapturingMovesOnUI();
                            }
                            else
                            {
                                await ListenForRemoteMove();
                            }
                        }

                        break;
                    }
                }
            }
        }

        private async Task ListenForRemoteMove()
        {
            Move mv = await opponent.ListenForMove();
            ApplyMoveToUI(mv);
            SwapTurn();
        }

        private void ForceCapturingMovesOnUI()
        {
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
                            lightChecker = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.graycheckerkinglight.jpg") };
                        else
                            lightChecker = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.redcheckerkinglight.jpg") };
                    }
                    else if (turn == (Color)0)
                        lightChecker = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.graycheckerlight.jpg") };
                    else
                        lightChecker = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.redcheckerlight.jpg") };
                    lightChecker.Clicked += ClickedGrid;
                    boardGrid.Children.Add(lightChecker, capturer.location.xCoord, capturer.location.yCoord);
                }
            }
        }

        private void ApplyMoveToUI(Move m)
        {
            ImageButton movedChecker;
            if (gameBoard.IsKingSpace(m.endLoc, turn) || m.movingPiece.rank == Rank.King)
            {
                m.movingPiece.rank = Rank.King;
                if (turn == (Color)0)
                    movedChecker = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.graycheckerking.jpg") };
                else
                    movedChecker = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.redcheckerking.jpg") };
            }
            else if (turn == (Color)0)
                movedChecker = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.graychecker.jpg") };
            else
                movedChecker = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.redchecker.jpg") };
            movedChecker.Clicked += ClickedGrid;
            ImageButton emptyBoard = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.blackboard.jpg") };
            emptyBoard.Clicked += ClickedGrid;
            boardGrid.Children.Add(emptyBoard, m.movingPiece.location.xCoord, m.movingPiece.location.yCoord);
            boardGrid.Children.Add(movedChecker, m.endLoc.xCoord, m.endLoc.yCoord);
            foreach (Piece capt in m.capturedPieces)
            {
                ImageButton anotherEmptyBoard = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.blackboard.jpg") };
                boardGrid.Children.Add(anotherEmptyBoard, capt.location.xCoord, capt.location.yCoord);
            }
            gameBoard.ApplyMove(m);
            foreach (Piece light in highlightPieces)
            {
                ImageButton normChecker;
                if (light.rank == Rank.King)
                {
                    if (turn == (Color)0)
                        normChecker = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.graycheckerking.jpg") };
                    else
                        normChecker = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.redcheckerking.jpg") };
                }
                else if (turn == (Color)0)
                    normChecker = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.graychecker.jpg") };
                else
                    normChecker = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.redchecker.jpg") };
                normChecker.Clicked += ClickedGrid;
                boardGrid.Children.Add(normChecker, light.location.xCoord, light.location.yCoord);
            }
            highlightPieces.Clear();

            if (turn == localColor)
            {
                moveRecs.Remove(m);
            }

            foreach (Move move in moveRecs)
            {
                emptyBoard = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.blackboard.jpg") };
                emptyBoard.Clicked += ClickedGrid;
                if (gameBoard.FindPiece(move.endLoc) == null)
                    boardGrid.Children.Add(emptyBoard, move.endLoc.xCoord, move.endLoc.yCoord);
            }
            moveRecs.Clear();
        }

        private void SwapTurn()
        {
            if (gameBoard.IsInWinState())
            {
                Color winner = gameBoard.GetWinner();
                if (winner == (Color)0)
                    turnTracker.Text = "WINNER! BLACK";
                else
                    turnTracker.Text = "WINNER! RED";
            }
            else if (turn == (Color)0)
            {
                turn = (Color)1;
                turnTracker.Text = "Red's Turn";
            }
            else
            {
                turn = (Color)0;
                turnTracker.Text = "Black's Turn";
            }
        }

        private async void WaitAndApplyRemoteMove()
        {
            Move m = await opponent.ListenForMove();
            gameBoard.ApplyMove(m);
        }
    }
}
