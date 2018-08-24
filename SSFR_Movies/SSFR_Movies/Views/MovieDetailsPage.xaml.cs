using CommonServiceLocator;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
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

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var t3 = ScrollTrailer.ScrollToAsync(-200, 0, true);
            
            await Task.WhenAll(t3);

            var movie = (Result)BindingContext;

            var video = await ServiceLocator.Current.GetInstance<ApiClient>().GetMovieVideosAsync((int)movie.Id);

            if (video.Results.Count() == 0)
            {
                ScrollTrailer.IsVisible = false;
            }
            else
            {
                ScrollTrailer.IsVisible = true;
            }

        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            GC.Collect();
        }

        private async void PlayTrailer_Tapped(object sender, EventArgs e)
        {
            await Task.Yield();

            var movie = (Result)BindingContext;

            var video = await ServiceLocator.Current.GetInstance<ApiClient>().GetMovieVideosAsync((int)movie.Id);

            Device.OpenUri(new Uri("vnd.youtube://watch/" + video.Results.Where(v => v.Type == "Trailer").FirstOrDefault().Key));
        }

        private async void TitleTapped(object sender, EventArgs e)
        {
            var tilte = ((Label)sender).Text;

            if (tilte.Length > 25)
            {
                var t = PosterPath.FadeTo(0, 500, Easing.Linear);

                var t3 = PosterPath.FadeTo(1, 1000, Easing.Linear);

                await Task.WhenAll(t, t3);
            }
        }
    }
}