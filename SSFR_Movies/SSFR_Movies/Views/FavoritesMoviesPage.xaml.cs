//using CommonServiceLocator;
using Realms;
using Splat;
using SSFR_Movies.Helpers;
//using SSFR_Movies.Data;
using SSFR_Movies.Models;
using SSFR_Movies.ViewModels;
using SSFR_Movies.Views.DataTemplateSelectors;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    /// <summary>
    /// FavoriteMoviesPage Code Behind
    /// </summary>
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
   
    public partial class FavoritesMoviesPage : ContentPage
    {
        readonly FavoriteMoviesPageViewModel vm;
        readonly ToolbarItem searchToolbarItem = null;

        public FavoritesMoviesPage()
        {
            InitializeComponent();

            vm = Locator.Current.GetService<FavoriteMoviesPageViewModel>();

            BindingContext = vm;

            SetCollectionViewItemTemplate();

            searchToolbarItem = new ToolbarItem()
            {
                Text = "Search",
                IconImageSource = "Search.png",
                Priority = 0,
                Command = new AsyncCommand(async () =>
                {
                    await Shell.Current.GoToAsync("/Search", false);
                })
            };

            ToolbarItems.Add(searchToolbarItem);

            SubscribeToMessage();

            SetListOrientationLayout();
        }

        void SetCollectionViewItemTemplate()
        {
            MoviesList.ItemTemplate = new SelectedFavoriteMovieTemplateSelector();
            MoviesList.Bind(CollectionView.ItemsSourceProperty, nameof(FavoriteMoviesPageViewModel.FavMoviesList));
        }

        private void SetListOrientationLayout()
        {
            MoviesList.ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
            {
                SnapPointsAlignment = SnapPointsAlignment.Start,
                SnapPointsType = SnapPointsType.MandatorySingle
            };
        }

        private void SubscribeToMessage()
        {
            MessagingCenter.Subscribe<MovieDetailsPage>(this, "ClearSelection", (e) =>
            {
                MoviesList.SelectedItem = null;
            });
        }

        private async void MovieSelected(object sender, SelectionChangedEventArgs e)
        {
            ResultSingleton.SetInstance(MoviesList.SelectedItem as Result);
            await Shell.Current.GoToAsync("/MovieDetails", true);
        }

        /// <summary>
        /// To animate the Quit from Favorite list icon..
        /// </summary>
        /// <param name="sender">the incoming object </param>
        /// <param name="e">the event arguments in that object</param>
        private async void UnPin_Tapped(object sender, EventArgs e)
        {
            var img = sender as Image;

            await Task.WhenAll(img.ScaleTo(2, 500, Easing.BounceOut), img.ScaleTo(1, 250, Easing.BounceIn));
        }
    }
}
