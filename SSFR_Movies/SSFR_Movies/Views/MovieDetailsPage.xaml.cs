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
	}
}