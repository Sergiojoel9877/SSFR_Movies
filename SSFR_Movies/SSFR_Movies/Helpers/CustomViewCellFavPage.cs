﻿using CommonServiceLocator;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using Plugin.Connectivity;
using SSFR_Movies.Data;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using SSFR_Movies.ViewModels;
using SSFR_Movies.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Debug = System.Diagnostics;

namespace SSFR_Movies.Helpers
{
    [Preserve(AllMembers = true)]
    public class CustomViewCellFavPage : ViewCell
    {
        #region Controls
        private CachedImage blurCachedImage = null;
        //private Image blurCachedImage = null;
        private CachedImage cachedImage = null;
        //private Image cachedImage = null;
        private FlexLayout FlexLayout = null;
        private StackLayout Container = null;
        private StackLayout SubContainer = null;
        private AbsoluteLayout absoluteLayout = null;
        private StackLayout panelContainer = null;
        private Frame FrameUnderImages = null;
        private Grid gridInsideFrame = null;
        private ScrollView scrollTitle = null;
        private Label releaseDate = null;
        public Label title = null;
        private Image unPinFromFavList = null;
        private Label quitFromFavList = null;
        private StackLayout compat = null;
        private MenuItem QuitFromFavListCtxAct = null;
        private TapGestureRecognizer tap = null;
        private TapGestureRecognizer imageTapped = null;

        #endregion

        public CustomViewCellFavPage()
        {

            BindingContext = BindingContext;

            FlexLayout = new FlexLayout()
            {
                Direction = FlexDirection.Column,
                AlignContent = FlexAlignContent.Center
            };

            Container = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Center,

                VerticalOptions = LayoutOptions.FillAndExpand
            };

            SubContainer = new StackLayout()
            {
                Margin = new Thickness(16, 0, 16, 0),
                HeightRequest = 280,
                BackgroundColor = Color.FromHex("#44312D2D"),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            absoluteLayout = new AbsoluteLayout()
            {
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            List<FFImageLoading.Work.ITransformation> Blur = new List<FFImageLoading.Work.ITransformation>
            {
                new BlurredTransformation(15)
            };

            blurCachedImage = new CachedImage()
            {
                BitmapOptimizations = true,
                DownsampleToViewSize = true,
                HeightRequest = 350,
                CacheType = FFImageLoading.Cache.CacheType.Disk,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Scale = 3,
                LoadingPlaceholder = "Loading.png",
                VerticalOptions = LayoutOptions.FillAndExpand,
                WidthRequest = 350,
                Transformations = Blur
            };

            cachedImage = new CachedImage()
            {
                BitmapOptimizations = true,
                DownsampleToViewSize = true,
                HeightRequest = 280,
                CacheType = FFImageLoading.Cache.CacheType.Disk,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                WidthRequest = 280,
                LoadingPriority = FFImageLoading.Work.LoadingPriority.Highest
            };

            panelContainer = new StackLayout()
            {
                HeightRequest = 100,
                HorizontalOptions = LayoutOptions.Center,
            };

            FrameUnderImages = new Frame()
            {
                BackgroundColor = Color.FromHex("#44312D2D"),
                CornerRadius = 5,
                Margin = new Thickness(16, 0, 16, 0),
                HorizontalOptions = LayoutOptions.Center,
            };

            ColumnDefinitionCollection columnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition() { Width = GridLength.Star},
                    new ColumnDefinition() { Width = GridLength.Star}
                };

            RowDefinitionCollection rowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() { Height = GridLength.Star},
                    new RowDefinition() { Height = GridLength.Star}
                };

            gridInsideFrame = new Grid()
            {
                ColumnDefinitions = columnDefinitions,
                RowDefinitions = rowDefinitions
            };

            scrollTitle = new ScrollView()
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                VerticalScrollBarVisibility = ScrollBarVisibility.Never,
                Orientation = ScrollOrientation.Horizontal
            };

            title = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.White,
                LineBreakMode = LineBreakMode.TailTruncation,
                Margin = new Thickness(16, 0, 0, 0),
                FontFamily = "Arial",
                FontAttributes = FontAttributes.Bold,

            };
            title.SetBinding(Label.TextProperty, "Title");

            scrollTitle.Content = title;

            releaseDate = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = Color.FromHex("#65FFFFFF"),
                LineBreakMode = LineBreakMode.NoWrap,
                Margin = new Thickness(16, 0, 0, 0),
                FontFamily = "Arial",
                FontAttributes = FontAttributes.Bold
            };
            releaseDate.SetBinding(Label.TextProperty, "ReleaseDate");

            compat = new StackLayout()
            {
                HeightRequest = 50
            };

            unPinFromFavList = new Image()
            {
                HeightRequest = 40,
                WidthRequest = 40
            };

            quitFromFavList = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.End,
                Text = "Quit from Fav. List",
                FontAttributes = FontAttributes.Bold
            };

            compat.Children.Add(unPinFromFavList);
            compat.Children.Add(quitFromFavList);

            gridInsideFrame.Children.Add(scrollTitle, 0, 0);
            Grid.SetColumnSpan(scrollTitle, 2);
            gridInsideFrame.Children.Add(releaseDate, 0, 1);
            gridInsideFrame.Children.Add(compat, 2, 0);
            Grid.SetRowSpan(compat, 2);

            AbsoluteLayout.SetLayoutBounds(blurCachedImage, new Rectangle(.5, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(blurCachedImage, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(cachedImage, new Rectangle(.5, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(cachedImage, AbsoluteLayoutFlags.All);

            FrameUnderImages.Content = gridInsideFrame;

            absoluteLayout.Children.Add(blurCachedImage);
            absoluteLayout.Children.Add(cachedImage);
            CompressedLayout.SetIsHeadless(absoluteLayout, true);

            panelContainer.Children.Add(FrameUnderImages);

            SubContainer.Children.Add(absoluteLayout);
            CompressedLayout.SetIsHeadless(SubContainer, true);
            Container.Children.Add(SubContainer);
            Container.Children.Add(panelContainer);

            FlexLayout.Children.Add(Container);

            QuitFromFavListCtxAct = new MenuItem { Text = "Quit from Favorites", Icon = "StarEmpty.png" };

            QuitFromFavListCtxAct.Clicked += QuitFromFavList;

            ContextActions.Add(QuitFromFavListCtxAct);

            tap = new TapGestureRecognizer();

            imageTapped = new TapGestureRecognizer();

            tap.Tapped += QuitFromFavListTap;

            imageTapped.Tapped += PosterTapped;

            compat.GestureRecognizers.Add(tap);

            cachedImage.GestureRecognizers.Add(imageTapped);

            View = FlexLayout;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var result = BindingContext as Result;

            await result.IsPresentInFavList(unPinFromFavList, result.Id).ConfigureAwait(false);

        }

        private void PosterTapped(object sender, EventArgs e)
        {
            var movie = BindingContext as Result;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new MovieDetailsPage(movie), true);
            });
        }

        protected override async void OnBindingContextChanged()
        {

            unPinFromFavList.Source = "StarEmpty.png";

            blurCachedImage.Source = null;

            cachedImage.Source = null;

            var item = BindingContext as SSFR_Movies.Models.Result;

            ExecuteAction(async () =>
            {
                await item.IsPresentInFavList(unPinFromFavList, item.Id);

                if (title.Text.Length >= 20)
                {
                    title.SetAnimation();
                }
            });

            if (item == null)
            {
                return;
            }

            Uri bimg, pimg;

            Uri.TryCreate("https://image.tmdb.org/t/p/w1066_and_h600_bestv2" + item.BackdropPath, UriKind.Absolute, out bimg);

            Uri.TryCreate("https://image.tmdb.org/t/p/w370_and_h556_bestv2" + item.PosterPath, UriKind.Absolute, out pimg);

            Task<ImageSource> bimg_result = Task<ImageSource>.Factory.StartNew(() => ImageSource.FromUri(bimg));

            Task<ImageSource> pimg_result = Task<ImageSource>.Factory.StartNew(() => ImageSource.FromUri(pimg));

            blurCachedImage.Source = await bimg_result;

            cachedImage.Source = await pimg_result;

            base.OnBindingContextChanged();
        }
        
        async void ExecuteAction(Func<Task> exe)
        {
            await Task.Run(() => { exe(); }).ConfigureAwait(false);
        }
        
        private async void QuitFromFavListTap(object sender, EventArgs e)
        {
            await Task.Yield();

            await unPinFromFavList.ScaleTo(1.50, 500, Easing.BounceOut);

            if (sender != null)
            {

                var movie = BindingContext as Result;

                //Verify if internet connection is available
                if (!CrossConnectivity.Current.IsConnected)
                {
                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                        return false;
                    });
                    return;
                }

                try
                {
                    var deleteMovie = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().DeleteEntity(movie).ConfigureAwait(false);

                    if (deleteMovie)
                    {
                        ServiceLocator.Current.GetInstance<FavoriteMoviesPageViewModel>().FavMoviesList.Remove(movie);

                        MessagingCenter.Send(this, "RefreshList", true);

                        await unPinFromFavList.ScaleTo(1, 500, Easing.BounceIn);
                    }
                
                }
                catch (Exception e15)
                {
                    Debug.Debug.WriteLine("Error: " + e15.InnerException);
                }
            }
        }

        private async void QuitFromFavList(object sender, EventArgs e)
        {

            if (sender is MenuItem opt)
            {
                var movie = opt.BindingContext as Result;

                //Verify if internet connection is available
                if (!CrossConnectivity.Current.IsConnected)
                {
                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        DependencyService.Get<IToast>().LongAlert("Please be sure that your device has an Internet connection");
                        return false;
                    });
                    return;
                }

                try
                {

                    var deleteMovie = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().DeleteEntity(movie).ConfigureAwait(false);

                    if (deleteMovie)
                    {
                        ServiceLocator.Current.GetInstance<FavoriteMoviesPageViewModel>().FavMoviesList.Remove(movie);

                        await unPinFromFavList.ScaleTo(1, 500, Easing.BounceIn);

                        MessagingCenter.Send(this, "RefreshList", true);

                        DependencyService.Get<IToast>().LongAlert("Removed Successfully, The movie " + movie.Title + " was removed from your favorite list!");

                        await SpeakNow("Removed Successfully");

                        Vibration.Vibrate(0.5);

                    }
                }
                catch (Exception)
                {

                }
            }
        }
        public async Task SpeakNow(string msg)
        {
            var settings = new SpeechOptions()
            {
                Volume = 1f,
                Pitch = 1.0f
            };

            await TextToSpeech.SpeakAsync(msg, settings);
        }
    }
}
