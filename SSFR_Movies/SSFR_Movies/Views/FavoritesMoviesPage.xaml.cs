//using CommonServiceLocator;
using Realms;
using Splat;
using SSFR_Movies.Helpers;
//using SSFR_Movies.Data;
using SSFR_Movies.Models;
using SSFR_Movies.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    /// <summary>
    /// FavoriteMoviesPage Code Behind
    /// </summary>
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FavoritesMoviesPage : ContentPage
    {
        readonly FavoriteMoviesPageViewModel vm;
        readonly ToolbarItem searchToolbarItem = null;

        public FavoritesMoviesPage()
        {
            InitializeComponent();

            vm = Locator.Current.GetService<FavoriteMoviesPageViewModel>();

            BindingContext = vm;

            SetVisibility();

            searchToolbarItem = new ToolbarItem()
            {
                Text = "Search",
                Icon = "Search.png",
                Priority = 0,

                Command = new Command(async () =>
                {
                    await Navigation.PushAsync(new SearchPage(), true);
                })
            };

            ToolbarItems.Add(searchToolbarItem);

            MoviesList.SelectionChangedCommand = new Command(MovieSelected);

            SubscribeToMessage();
        }

        private void SubscribeToMessage()
        {
            MessagingCenter.Subscribe<MovieDetailsPage, bool>(this, "Refresh", (s, e) =>
            {
                if (e)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var estado = await vm.FillMoviesList(null);

                        if (estado.Key == 'r')
                        {
                            MoviesList.IsVisible = true;
                            UnPin.IsVisible = false;
                            Message.IsVisible = false;
                            MoviesList.ItemsSource = estado.Value;
                            MoviesList.SelectedItem = null;
                        }
                        else if (estado.Key == 'v')
                        {
                            MoviesList.IsVisible = false;
                            UnPin.IsVisible = true;
                            Message.IsVisible = true;
                            MoviesList.SelectedItem = null;
                        }
                    });
                }
            });

            MessagingCenter.Subscribe<CustomViewCellFavPage, bool>(this, "Refresh", (s, e) =>
            {
                if (e)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var estado = await vm.FillMoviesList(null);

                        if (estado.Key == 'r')
                        {
                            MoviesList.IsVisible = true;
                            UnPin.IsVisible = false;
                            Message.IsVisible = false;
                            MoviesList.ItemsSource = estado.Value;
                            MoviesList.SelectedItem = null;
                        }
                        else if (estado.Key == 'v')
                        {
                            MoviesList.IsVisible = false;
                            UnPin.IsVisible = true;
                            Message.IsVisible = true;
                            MoviesList.SelectedItem = null;
                        }
                    });
                }
            });

            MessagingCenter.Subscribe<CustomViewCell, bool>(this, "Refresh", (s, e) =>
            {
                if (e)
                {
                    Device.BeginInvokeOnMainThread(async ()=>
                    {
                        var estado = await vm.FillMoviesList(null);

                        if (estado.Key == 'r')
                        {
                            MoviesList.IsVisible = true;
                            UnPin.IsVisible = false;
                            Message.IsVisible = false;
                            MoviesList.ItemsSource = estado.Value;
                            MoviesList.SelectedItem = null;
                        }
                        else if (estado.Key == 'v')
                        {
                            MoviesList.IsVisible = false;
                            UnPin.IsVisible = true;
                            Message.IsVisible = true;
                            MoviesList.SelectedItem = null;
                        }
                    });
                }
            });
        }

        private void MovieSelected()
        {
            if (MoviesList.SelectedItem != null)
            {
                var movie = MoviesList.SelectedItem as Result;

                Result resultSingleton = ResultSingleton.SetInstance(movie);

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.GoToAsync("app://ssfr.com/MovieDetails", true);
                });
            }
        }

        private async void SetVisibility()
        {
            var realm = await Realm.GetInstanceAsync();

            var movies_db = realm.All<Result>().Where(x => x.FavoriteMovie == "Star.png").ToList();

            UnPin.IsVisible = movies_db.Count() == 0 ? true : false;

            Message.IsVisible = UnPin.IsVisible == true ? true : false;

            MoviesList.IsVisible = Message.IsVisible == true ? false : true;
        }

        private async void QuitVisibility()
        {
            var realm = await Realm.GetInstanceAsync();

            var movies_db = realm.All<Result>().ToList();

            UnPin.IsVisible = movies_db.Count() != 0 ? false : true;

            Message.IsVisible = UnPin.IsVisible == true ? true : false;

            MoviesList.IsVisible = Message.IsVisible == true ? false : true;

        }

        /// <summary>
        /// To animate the Quit from Favorite list icon..
        /// </summary>
        /// <param name="sender">the incoming object </param>
        /// <param name="e">the event arguments in that object</param>
        private async void UnPin_Tapped(object sender, EventArgs e)
        {
            var img = sender as Image;

            await img.ScaleTo(2, 500, Easing.BounceOut);

            await img.ScaleTo(1, 250, Easing.BounceIn);
        }
    }
}
