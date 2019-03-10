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
	public partial class HomeScreen : ContentPage
	{
		public HomeScreen ()
		{
			InitializeComponent ();
		}

        async void HostButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HostPage());
        }

        async void JoinButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new JoinPage());
        }

       /* async void ExitButtonClicked(object sender, EventArgs e)
        {
            
        } */

    }
}