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
        private List<Location> highlightLocs;
        private double timeout;
        private Color turn, localColor;
        private Board gameBoard;
        private Connection opponent;

        public MainPage(Connection opp, Color col)
        {
            InitializeComponent();
            gameBoard = new Board();
            gameBoard.NewCheckersGame();
            moveRecs = new List<Move>();
            highlightLocs = new List<Location>();
            turn = (Color)0;
            localColor = col;
            if (turn == localColor)
                turnTracker.Text = "Your Turn";
            else
                turnTracker.Text = "Opponent's Turn";
            opponent = opp;
        }

        protected override async void OnAppearing()
        {
            if (localColor != turn)
            {
                await ListenForRemoteMove();
            }
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
                if (highlightLocs.Count == 0 || highlightLocs.Contains(p.location))
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
                            await ListenForRemoteMove();
                        }

                        break;
                    }
                }
            }
        }

        private async Task ListenForRemoteMove()
        {
            Task<Move> result = Task.Run(() => opponent.ListenForMove());
            Move mv = await result;

            if (mv.forfeit)
            {
                if (localColor == Color.Black)
                {
                    turnTracker.Text = "RED FORFEITS";
                }
                else
                {
                    turnTracker.Text = "BLACK FORFEITS";
                }
                return;
            }

            ApplyMoveToUI(mv);
            SwapTurn();
            if (turn == localColor)
            {
                ForceCapturingMovesOnUI();
            }
        }

        private void ForceCapturingMovesOnUI()
        {
            List<Move> captMoves = gameBoard.FindCapturingMoves(turn);
            if (captMoves.Count > 0)
            {
                foreach (Move captMove in captMoves)
                {
                    highlightLocs.Add(captMove.startLoc);
                    ImageButton lightChecker;
                    if (captMove.pieceRank == Rank.King)
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
                    boardGrid.Children.Add(lightChecker, captMove.startLoc.xCoord, captMove.startLoc.yCoord);
                }
            }
        }

        private void ApplyMoveToUI(Move m)
        {
            foreach (Location light in highlightLocs)
            {
                ImageButton normChecker;
                if (gameBoard.FindPiece(light).rank == Rank.King)
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
                boardGrid.Children.Add(normChecker, light.xCoord, light.yCoord);
            }
            highlightLocs.Clear();

            ImageButton movedChecker;
            if (gameBoard.IsKingSpace(m.endLoc, turn) || m.pieceRank == Rank.King)
            {
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
            boardGrid.Children.Add(emptyBoard, m.startLoc.xCoord, m.startLoc.yCoord);
            boardGrid.Children.Add(movedChecker, m.endLoc.xCoord, m.endLoc.yCoord);
            foreach (Location capt in m.capturedPieceLocs)
            {
                ImageButton anotherEmptyBoard = new ImageButton { Source = ImageSource.FromResource("XamarinCheckers.Assets.blackboard.jpg") };
                anotherEmptyBoard.Clicked += ClickedGrid;
                boardGrid.Children.Add(anotherEmptyBoard, capt.xCoord, capt.yCoord);
            }
            gameBoard.ApplyMove(m);

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
                turn = (Color)1;
            else
                turn = (Color)0;
            captCounter.Text = "Captured Pieces: " + gameBoard.CapturedCount(localColor);
            if (turn == localColor)
                turnTracker.Text = "Your Turn";
            else
                turnTracker.Text = "Opponent's Turn";
        }

        private async void Forfeit_Clicked(object sender, EventArgs e)
        {
            Move m = new Move();
            m.forfeit = true;
            await opponent.SendMove(m);
            if (localColor == Color.Black)
            {
                turnTracker.Text = "BLACK FORFEITS";
            }
            else
            {
                turnTracker.Text = "RED FORFEITS";
            }
        }

        private async void WaitAndApplyRemoteMove()
        {
            Move m = await opponent.ListenForMove();
            gameBoard.ApplyMove(m);
        }
    }
}
