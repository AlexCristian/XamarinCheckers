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
        private GameSession game = new GameSession();

        public MainPage()
        {
            InitializeComponent();

        }

        private void clickedGrid(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Location square = new Location(Grid.GetColumn(btn), Grid.GetRow(btn));
            Console.WriteLine("Clicked on " + Grid.GetColumn(btn) + ", " + Grid.GetRow(btn));
            List<Location> highlightCells = game.processSelection(square);
            foreach (Location l in highlightCells)
            {
                Console.WriteLine("Trying to highlight possible move at " + l.xCoord + "," + l.yCoord);
                boardGrid.Children.Add(new Button { BackgroundColor = Xamarin.Forms.Color.Yellow }, l.xCoord, l.yCoord);
            }
            game.swapTurn();
        }
    }
}
