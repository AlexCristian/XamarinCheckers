using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinCheckers
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class JoinPage : ContentPage
	{
		public JoinPage ()
		{
			InitializeComponent ();
		}

        private async void JoinButtonClicked(object sender, EventArgs e)
        {
            await ConnectToOpponent(ipaddress.Text);
        }

        private async Task ConnectToOpponent(string ip)
        {
            Task<Connection> result = Task.Run(() => Network.ConnectWithOpponent(ip));
            Connection opponent = await result;
            await Navigation.PushAsync(new MainPage(opponent, Color.Red));
        }
    }
}