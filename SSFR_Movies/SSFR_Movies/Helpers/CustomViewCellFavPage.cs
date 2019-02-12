using CommonServiceLocator;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using Plugin.Connectivity;
using Realms;
//using SSFR_Movies.Data;
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
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public class CustomViewCellFavPage : FlexLayout
    {
        #region Controls
        private Lazy<Image> blurCachedImage = null;
        //private Image blurCachedImage = null;
        private Lazy<Image> cachedImage = null;
        //private Image cachedImage = null;
        private Lazy<StackLayout> Container = null;
        private Lazy<StackLayout> SubContainer = null;
        private Lazy<AbsoluteLayout> absoluteLayout = null;
        private Lazy<StackLayout> panelContainer = null;
        private Lazy<Frame> FrameUnderImages = null;
        private Lazy<Grid> gridInsideFrame = null;
        private Lazy<ScrollView> scrollTitle = null;
        private Lazy<Label> releaseDate = null;
        public Lazy<Label> title = null;
        private Lazy<Image> unPinFromFavList = null;
        private Lazy<StackLayout> compat = null;
        private TapGestureRecognizer tap = null;
        private TapGestureRecognizer imageTapped = null;
        #endregion

        public CustomViewCellFavPage()
        {
            HeightRequest = 300;
            Direction = FlexDirection.Column;
            Margin = 16;
            AlignContent = FlexAlignContent.Center;

            Container = new Lazy<StackLayout>(() => new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Center,

                VerticalOptions = LayoutOptions.FillAndExpand
            });

            SubContainer = new Lazy<StackLayout>(() => new StackLayout()
            {
                Margin = new Thickness(16, 0, 16, 0),
                HeightRequest = 280,
                BackgroundColor = Color.FromHex("#44312D2D"),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            });

            absoluteLayout = new Lazy<AbsoluteLayout>(() => new AbsoluteLayout()
            {
                VerticalOptions = LayoutOptions.FillAndExpand
            });

            List<FFImageLoading.Work.ITransformation> Blur = new List<FFImageLoading.Work.ITransformation>
            {
                new BlurredTransformation(15)
            };

            blurCachedImage = new Lazy<Image>(() => new Image()
            {
                //BitmapOptimizations = true,
                //DownsampleToViewSize = true,
                HeightRequest = 330,
                Opacity = 0.6,
                //FadeAnimationEnabled = true,
                //FadeAnimationForCachedImages = true,
                //RetryCount = 5,
                //RetryDelay = 2000,
                //CacheType = FFImageLoading.Cache.CacheType.Disk,
                //LoadingPriority = FFImageLoading.Work.LoadingPriority.Highest,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Scale = 3,
                //LoadingPlaceholder = "Loading.png",
                VerticalOptions = LayoutOptions.FillAndExpand,
                WidthRequest = 330
                //Transformations = Blur
            });
            blurCachedImage.Value.SetBinding(Image.SourceProperty, "BackdropPath", stringFormat: $"https://image.tmdb.org/t/p/w1066_and_h600_bestv2{0}");

            cachedImage = new Lazy<Image>(() => new Image()
            {
                //BitmapOptimizations = true,
                //DownsampleToViewSize = true,
                //FadeAnimationEnabled = true,
                //FadeAnimationForCachedImages = true,
                //RetryCount = 5,
                //RetryDelay = 2000,
                HeightRequest = 280,
                //CacheType = FFImageLoading.Cache.CacheType.Disk,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                WidthRequest = 280,
                //LoadingPriority = FFImageLoading.Work.LoadingPriority.Highest
            });
            cachedImage.Value.SetBinding(Image.SourceProperty, "PosterPath", stringFormat: $"https://image.tmdb.org/t/p/w370_and_h556_bestv2{0}");

            panelContainer = new Lazy<StackLayout>(() => new StackLayout()
            {
                HeightRequest = 125,
                HorizontalOptions = LayoutOptions.Center,
            });

            FrameUnderImages = new Lazy<Frame>(() => new Frame()
            {
                BackgroundColor = Color.FromHex("#44312D2D"),
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.Center
            });

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

            gridInsideFrame = new Lazy<Grid>(() => new Grid()
            {
                ColumnDefinitions = columnDefinitions,
                RowDefinitions = rowDefinitions
            });

            scrollTitle = new Lazy<ScrollView>(() => new ScrollView()
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                VerticalScrollBarVisibility = ScrollBarVisibility.Never,
                Orientation = ScrollOrientation.Horizontal
            });

            title = new Lazy<Label>(() => new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.White,
                LineBreakMode = LineBreakMode.TailTruncation,
                Margin = new Thickness(16, 0, 0, 0),
                FontFamily = "Arial",
                FontAttributes = FontAttributes.Bold
            });

            title.Value.SetBinding(Label.TextProperty, "Title");

            scrollTitle.Value.Content = title.Value;

            releaseDate = new Lazy<Label>(() => new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = Color.FromHex("#65FFFFFF"),
                LineBreakMode = LineBreakMode.NoWrap,
                Margin = new Thickness(16, 0, 0, 0),
                FontFamily = "Arial",
                FontAttributes = FontAttributes.Bold
            });
            releaseDate.Value.SetBinding(Label.TextProperty, "ReleaseDate");

            compat = new Lazy<StackLayout>(() => new StackLayout()
            {
                HeightRequest = 50
            });

            unPinFromFavList = new Lazy<Image>(() => new Image()
            {
                HeightRequest = 40,
                WidthRequest = 40,
                //BitmapOptimizations = true,
                //DownsampleToViewSize = true,
                //FadeAnimationEnabled = true,
                //FadeAnimationForCachedImages = true,
                //RetryCount = 5,
                //RetryDelay = 2000,
                //CacheType = FFImageLoading.Cache.CacheType.Disk,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                //LoadingPriority = FFImageLoading.Work.LoadingPriority.Highest
            });
            unPinFromFavList.Value.SetBinding(Image.SourceProperty, "FavoriteMovie");

            compat.Value.Children.Add(unPinFromFavList.Value);

            gridInsideFrame.Value.Children.Add(scrollTitle.Value, 0, 0);
            Grid.SetColumnSpan(scrollTitle.Value, 2);
            gridInsideFrame.Value.Children.Add(releaseDate.Value, 0, 1);
            gridInsideFrame.Value.Children.Add(compat.Value, 2, 1);

            AbsoluteLayout.SetLayoutBounds(blurCachedImage.Value, new Rectangle(.5, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(blurCachedImage.Value, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(cachedImage.Value, new Rectangle(.5, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(cachedImage.Value, AbsoluteLayoutFlags.All);

            FrameUnderImages.Value.Content = gridInsideFrame.Value;

            absoluteLayout.Value.Children.Add(blurCachedImage.Value);
            absoluteLayout.Value.Children.Add(cachedImage.Value);
            CompressedLayout.SetIsHeadless(absoluteLayout.Value, true);

            panelContainer.Value.Children.Add(FrameUnderImages.Value);

            SubContainer.Value.Children.Add(absoluteLayout.Value);
            CompressedLayout.SetIsHeadless(SubContainer.Value, true);
            Container.Value.Children.Add(SubContainer.Value);
            Container.Value.Children.Add(panelContainer.Value);

            Children.Add(Container.Value);

            tap = new TapGestureRecognizer();

            tap.Tapped += QuitFromFavListTap;

            imageTapped = new TapGestureRecognizer();
            
            imageTapped.Tapped += PosterTapped;

            absoluteLayout.Value.GestureRecognizers.Add(imageTapped);

            compat.Value.GestureRecognizers.Add(tap);
            
        }
        
        private void PosterTapped(object sender, EventArgs e)
        {
            var movie = BindingContext as Result;
            
            MessagingCenter.Send(this, "PushAsync");
           
        }

        protected override void OnBindingContextChanged()
        {
            blurCachedImage.Value.Source = null;

            cachedImage.Value.Source = null;

            var item = BindingContext as Result;

            Device.BeginInvokeOnMainThread(() =>
            {
                if (title.Value.Text.Length >= 15)
                {
                    title.Value.SetAnimation();
                }
            });

            if (item == null)
            {
                return;
            }

            blurCachedImage.Value.Source = "https://image.tmdb.org/t/p/w1066_and_h600_bestv2" + item.BackdropPath;

            cachedImage.Value.Source = "https://image.tmdb.org/t/p/w370_and_h556_bestv2" + item.PosterPath;

            base.OnBindingContextChanged();
        }
        
        void ExecuteAction(Func<Task> exe)
        {
            Task.Run(async () => { await exe(); });
        }
        
        private async void QuitFromFavListTap(object sender, EventArgs e)
        {
            await Task.Yield();

            await unPinFromFavList.Value.ScaleTo(1.50, 500, Easing.BounceOut);

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
                    var realm = await Realm.GetInstanceAsync();

                    var deleteMovie = realm.Find<Result>(movie.Id);

                    if (deleteMovie != null)
                    {
                        realm.Write(()=>
                        {
                            movie.FavoriteMovie = "StarEmpty.png";

                            realm.Add(movie, true);
                        });

                        ServiceLocator.Current.GetInstance<Lazy<FavoriteMoviesPageViewModel>>().Value.FavMoviesList.Value.Remove(movie);

                        MessagingCenter.Send(this, "Refresh", true);

                        await unPinFromFavList.Value.ScaleTo(1, 500, Easing.BounceIn);
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

                    //var deleteMovie = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().DeleteEntity(movie).ConfigureAwait(false);

                    //if (deleteMovie)
                    //{
                    //    ServiceLocator.Current.GetInstance<Lazy<FavoriteMoviesPageViewModel>>().Value.FavMoviesList.Value.Remove(movie);

                    //    await unPinFromFavList.Value.ScaleTo(1, 500, Easing.BounceIn);

                    //    MessagingCenter.Send(this, "RefreshList", true);

                    //    DependencyService.Get<IToast>().LongAlert("Removed Successfully, The movie " + movie.Title + " was removed from your favorite list!");

                    //    await SpeakNow("Removed Successfully");

                    //    Vibration.Vibrate(0.5);

                    //}
                }
                catch (Exception err)
                {
                    Debug.Debug.WriteLine($"Error: {err}");
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
