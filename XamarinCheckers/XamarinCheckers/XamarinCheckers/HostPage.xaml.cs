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
	public partial class HostPage : ContentPage
	{
		public HostPage ()
		{
			InitializeComponent ();
            ipaddress.Text = Network.GetDeviceIPAddress();

        }

        protected override async void OnAppearing()
        {
            await WaitForOpponent();
        }

        private async Task WaitForOpponent()
        {
            Task<Connection> result = Task.Run(() => Network.ListenForOpponent());
            Connection opponent = await result;
            await Navigation.PushAsync(new MainPage(opponent, Color.Black));
        }
    }
}