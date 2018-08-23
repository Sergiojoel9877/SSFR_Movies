using SSFR_Movies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MovieDetailsPage : ContentPage
	{
		public MovieDetailsPage (Result movie)
		{
			InitializeComponent ();

            BindingContext = movie;
		}

        private void PlayTrailer_Tapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("vnd.youtube://watch/z8h3LVb8cl8")); //TEST
        }
    }
}