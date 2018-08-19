using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GBH_Movies_Test.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MovieGenresBar : ContentView
	{
		public MovieGenresBar ()
		{
			InitializeComponent ();
            Scrollview.Orientation = ScrollOrientation.Horizontal;
        }
	}
}