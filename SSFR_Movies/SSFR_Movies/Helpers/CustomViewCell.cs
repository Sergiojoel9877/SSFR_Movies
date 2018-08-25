using CommonServiceLocator;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using Plugin.Connectivity;
using SSFR_Movies.Data;
using SSFR_Movies.Models;
using SSFR_Movies.ResourceDictionaries;
using SSFR_Movies.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSFR_Movies.Helpers
{
    public class CustomViewCell : ViewCell
    {
        #region Controls
        public CachedImage blurCachedImage = null;
        public CachedImage cachedImage = null;
        public FlexLayout FlexLayout = null;
        public StackLayout Container = null;
        public StackLayout SubContainer = null;
        public AbsoluteLayout absoluteLayout = null;
        public StackLayout panelContainer = null;
        public Frame FrameUnderImages = null;
        public Grid gridInsideFrame = null;
        public ScrollView scrollTitle = null;
        public Label releaseDate = null;
        public Label title = null;
        public Image pin2FavList = null;
        public Label add2FavList = null;
        public StackLayout compat = null;
        public MenuItem AddToFavListCtxAct = null;

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
                HorizontalOptions = LayoutOptions.FillAndExpand,
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
                VerticalOptions = LayoutOptions.FillAndExpand,
                LoadingPriority = FFImageLoading.Work.LoadingPriority.High,
                WidthRequest = 280,
                Transformations = Blur
            };

            cachedImage = new CachedImage()
            {
                BitmapOptimizations = true,
                DownsampleToViewSize = false,
                HeightRequest = 280,
                LoadingPlaceholder = "Recent.png",
                FadeAnimationEnabled = true,
                FadeAnimationForCachedImages = true,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.FillAndExpand,
                WidthRequest = 280,
                LoadingPriority = FFImageLoading.Work.LoadingPriority.Highest
            };

            panelContainer = new StackLayout()
            {
                HeightRequest = 100
            };

            FrameUnderImages = new Frame()
            {
                BackgroundColor = Color.FromHex("#44312D2D"),
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.FillAndExpand,
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
                LineBreakMode = LineBreakMode.NoWrap,
                Margin = new Thickness(16, 0, 0, 0),
                FontFamily = "Arial",
                FontAttributes = FontAttributes.Bold
            };

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

            compat = new StackLayout()
            {
                HeightRequest = 50
            };  

            pin2FavList = new Image()
            {
                HeightRequest = 35,
                Source = "StarEmpty.png",
                WidthRequest = 35
            };

            add2FavList = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.End,
                Text = "Push down to Add To Fav. List",
                FontAttributes = FontAttributes.Bold,
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
            panelContainer.Children.Add(FrameUnderImages);

            SubContainer.Children.Add(absoluteLayout);
            Container.Children.Add(SubContainer);
            Container.Children.Add(panelContainer);
            FlexLayout.Children.Add(Container);
            
            AddToFavListCtxAct = new MenuItem { Text = "Add To Favorites", Icon = "Star.png" };

            AddToFavListCtxAct.Clicked += AddToFavList;

            ContextActions.Add(AddToFavListCtxAct);

            View = FlexLayout;
        }

        protected async override void OnBindingContextChanged()
        {
            await Task.Yield();

            AddToFavListCtxAct.Clicked -= AddToFavList;

            ContextActions.Clear();

            blurCachedImage.Source = null;
            cachedImage.Source = null;

            var item = BindingContext as SSFR_Movies.Models.Result;

            if (item == null)
            {
                return;
            }

            AddToFavListCtxAct.Clicked += AddToFavList;

            ContextActions.Add(AddToFavListCtxAct);

            blurCachedImage.Source = item.PosterPath;
            cachedImage.Source = item.PosterPath;

            title.Text = item.Title;
            releaseDate.Text = item.ReleaseDate;

            var t4 = scrollTitle.ScrollToAsync(100, 0, true);
            var t5 = scrollTitle.ScrollToAsync(0, 0, true);
            var t6 = scrollTitle.ScrollToAsync(100, 0, true);

            await Task.WhenAll(t4, t5, t6);
            
            base.OnBindingContextChanged();
        }

        private async void AddToFavList(object sender, EventArgs e)
        {
      
            var opt = sender as MenuItem;

            if (opt != null)
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

                    }
                }
                catch (Exception)
                {
               
                }
            }
        }


        public async Task IsPresentInFavList(Result m)
        {
            var movieExists = await ServiceLocator.Current.GetInstance<DBRepository<Result>>().EntityExits(m.Id);

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
