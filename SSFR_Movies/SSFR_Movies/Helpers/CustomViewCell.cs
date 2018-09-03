using CommonServiceLocator;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using Plugin.Connectivity;
using SSFR_Movies.Data;
using SSFR_Movies.Models;
using SSFR_Movies.ResourceDictionaries;
using SSFR_Movies.Services;
using SSFR_Movies.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSFR_Movies.Helpers
{
    public class CustomViewCell : ViewCell
    {
        #region Controls
        private CachedImage blurCachedImage = null;
        private CachedImage cachedImage = null;
        private FlexLayout FlexLayout = null;
        private StackLayout Container = null;
        private StackLayout SubContainer = null;
        private AbsoluteLayout absoluteLayout = null;
        private StackLayout panelContainer = null;
        private Frame FrameUnderImages = null;
        private Grid gridInsideFrame = null;
        private ScrollView scrollTitle = null;
        private Label releaseDate = null;
        private Label title = null;
        private Image pin2FavList = null;
        private Label add2FavList = null;
        private StackLayout compat = null;
        private MenuItem AddToFavListCtxAct = null;
        private TapGestureRecognizer tap = null;
        private TapGestureRecognizer imageTapped = null;

        #endregion

        public CustomViewCell()
        {
            
            BindingContext = BindingContext;

            FlexLayout = new FlexLayout()
            {
                Direction = FlexDirection.Column,
                Margin = 16,
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
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            absoluteLayout = new AbsoluteLayout()
            {
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            List<FFImageLoading.Work.ITransformation> Blur = new List<FFImageLoading.Work.ITransformation>
            {
                new BlurredTransformation(60)
            };

            blurCachedImage = new CachedImage()
            {
                BitmapOptimizations = true,
                DownsampleToViewSize = true,
                HeightRequest = 280,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Scale = 2,
                LoadingPlaceholder = "Loading.png",
                VerticalOptions = LayoutOptions.FillAndExpand,
                LoadingPriority = FFImageLoading.Work.LoadingPriority.High,
                WidthRequest = 280,
                Transformations = Blur
            };

            cachedImage = new CachedImage()
            {
                BitmapOptimizations = true,
                DownsampleToViewSize = true,
                HeightRequest = 280,
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
                FontAttributes = FontAttributes.Bold
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

            pin2FavList = new Image()
            {
                HeightRequest = 40,
                WidthRequest = 40
            };
            
            add2FavList = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.End,
                Text = "Add To Fav. List",
                FontAttributes = FontAttributes.Bold
            };
                 
            compat.Children.Add(pin2FavList);
            compat.Children.Add(add2FavList);

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

            AddToFavListCtxAct = new MenuItem { Text = "Add To Favorites", Icon = "Star.png" };

            AddToFavListCtxAct.Clicked += AddToFavList;

            ContextActions.Add(AddToFavListCtxAct);
            
            tap = new TapGestureRecognizer();

            imageTapped = new TapGestureRecognizer();

            tap.Tapped += AddToFavListTap;

            imageTapped.Tapped += PosterTapped;

            compat.GestureRecognizers.Add(tap);

            cachedImage.GestureRecognizers.Add(imageTapped);

            View = FlexLayout;
            
        }

        private void PosterTapped(object sender, EventArgs e)
        {
            var movie = BindingContext as Result;

            Device.BeginInvokeOnMainThread( () =>
            {
                App.Current.MainPage.Navigation.PushAsync(new MovieDetailsPage(movie), true);
            });
        }

        protected override void OnBindingContextChanged()
        {
           
            pin2FavList.Source = "StarEmpty.png";

            blurCachedImage.Source = null;

            cachedImage.Source = null;

            var item = BindingContext as SSFR_Movies.Models.Result;

            if (item == null)
            {
                return;
            }
            
            blurCachedImage.Source = item.PosterPath;

            cachedImage.Source = item.PosterPath;

            //await Task.Factory.StartNew(async () =>
            //{
            //    await IsPresentInFavList(item);
            //});

            base.OnBindingContextChanged();
        }

        private async void AddToFavListTap(object sender, EventArgs e)
        {
            await Task.Yield();

            await pin2FavList.ScaleTo(1.50, 500, Easing.BounceOut);

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
                    var movieExists = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().EntityExits(movie.Id);

                    if (movieExists)
                    {

                        DependencyService.Get<IToast>().LongAlert("Oh no It looks like " + movie.Title + " already exits in your favorite list!");

                        await pin2FavList.ScaleTo(1, 500, Easing.BounceIn);
                    }
                    
                    var addMovie = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().AddEntity(movie);

                    if (addMovie)
                    {

                        DependencyService.Get<IToast>().LongAlert("Added Successfully, The movie " + movie.Title + " was added to your favorite list!");

                        await SpeakNow("Added Successfully");

                        Vibration.Vibrate(1);

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            pin2FavList.Source = "Star.png";
                        });

                        await pin2FavList.ScaleTo(1, 500, Easing.BounceIn);
                    }
                }
                catch (Exception e15)
                {

                }
            }
        }

        private async void AddToFavList(object sender, EventArgs e)
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
                    var movieExists = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().EntityExits(movie.Id);

                    if (movieExists)
                    {
                        DependencyService.Get<IToast>().LongAlert("Oh no It looks like " + movie.Title + " already exits in your favorite list!");
                    }

                    var addMovie = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().AddEntity(movie);

                    if (addMovie)
                    {

                        DependencyService.Get<IToast>().LongAlert("Added Successfully, The movie " + movie.Title + " was added to your favorite list!");

                        await SpeakNow("Added Successfully");

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
            var settings = new SpeakSettings()
            {
                Volume = 1f,
                Pitch = 1.0f
            };

            await TextToSpeech.SpeakAsync(msg, settings);
        }

        public async Task IsPresentInFavList(Result m)
        {
            bool movieExists = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().EntityExits(m.Id);

            if (movieExists)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    pin2FavList.Source = "Star.png";
                });
            }
        }
    }
}
