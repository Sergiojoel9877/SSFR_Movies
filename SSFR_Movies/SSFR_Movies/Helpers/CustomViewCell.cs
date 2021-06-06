using System;
using System.Collections.Generic;
using System.Diagnostics;
using FFImageLoading.Forms;
using FFImageLoading.Work;
using Realms;
using Splat;
using SSFR_Movies.Converters;
using SSFR_Movies.Models;
using SSFR_Movies.Services.Abstract;
using Xamarin.Forms;

namespace SSFR_Movies.Helpers
{
    [Preserve(AllMembers = true)]
    public class CustomViewCell : FlexLayout
    {
        #region Controls
        private readonly Lazy<AbsoluteLayout> absoluteLayout = null;
        private readonly Lazy<Frame> FrameUnderImages = null;
        private readonly Lazy<Grid> gridInsideFrame = null;
        private readonly Lazy<CachedImage> blurCachedImage = null;
        private readonly Lazy<Frame> FrameCover = null;
        private readonly Lazy<CachedImage> cachedImage = null;
        private readonly Lazy<Image> pin2FavList = null;
        public Lazy<Label> title = null;
        private List<ITransformation> Transformations { get; set; } = new()
        {
            new FFImageLoading.Transformations.BlurredTransformation(15)
        };
        private readonly Lazy<StackLayout> Container = null;
        private readonly Lazy<StackLayout> SubContainer = null;
        private readonly Lazy<StackLayout> panelContainer = null;
        private readonly Lazy<Label> releaseDate = null;
        private readonly Lazy<StackLayout> compat = null;
        private readonly TapGestureRecognizer tap = null;

        Result Result { get; }
        #endregion

        public CustomViewCell(Result result)
        {
            try
            {
                HeightRequest = 350;
                Direction = FlexDirection.Column;
                Margin = 16;
                AlignContent = FlexAlignContent.Center;

                Container = new Lazy<StackLayout>(() => new StackLayout()
                {
                    Margin = 16,
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

                blurCachedImage = new Lazy<CachedImage>(() => new CachedImage()
                {
                    HeightRequest = 300,
                    WidthRequest = 300,
                    CacheDuration = new TimeSpan(20000),
                    DownsampleToViewSize = true,
                    LoadingPlaceholder = "Loading.png",
                    Aspect = Aspect.AspectFill,
                    Scale = 3,
                    ErrorPlaceholder = "About.png",
                    BitmapOptimizations = true,
                    Transformations = Transformations,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                });
                blurCachedImage.Value.SetBinding(CachedImage.SourceProperty, new Binding("BackdropPath", BindingMode.Default, new BackgroundImageUrlConverter()));

                cachedImage = new Lazy<CachedImage>(() => new CachedImage()
                {
                    Aspect = Aspect.AspectFill,
                    LoadingPlaceholder = "NoInternet.png",
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                });
                cachedImage.Value.SetBinding(CachedImage.SourceProperty, new Binding("PosterPath", BindingMode.Default, new PosterImageUrlConverter()));

                FrameCover = new Lazy<Frame>(() => new Frame()
                {
                    IsClippedToBounds = true,
                    Margin = new Thickness(0, 0, 0, 0),
                    HasShadow = true,
                    BorderColor = Color.FromHex("#00000000"),
                    Padding = new Thickness(0, 0, 0, 0),
                    BackgroundColor = Color.FromHex("#00000000"),
                    CornerRadius = 15
                });

                panelContainer = new Lazy<StackLayout>(() => new StackLayout()
                {
                    HeightRequest = 125,
                    HorizontalOptions = LayoutOptions.Center,
                });

                FrameUnderImages = new Lazy<Frame>(() => new Frame()
                {
                    BackgroundColor = Color.FromHex("#44312D2D"),
                    CornerRadius = 5,
                    HasShadow = true,
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

                pin2FavList = new Lazy<Image>(() => new Image()
                {
                    HeightRequest = 40,
                    WidthRequest = 40,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                });
                pin2FavList.Value.SetBinding(Image.SourceProperty, "FavoriteMovie");

                FrameCover.Value.Content = cachedImage.Value;

                compat.Value.Children.Add(pin2FavList.Value);

                gridInsideFrame.Value.Children.Add(title.Value, 0, 0);
                Grid.SetColumnSpan(title.Value, 3);
                gridInsideFrame.Value.Children.Add(releaseDate.Value, 0, 1);
                gridInsideFrame.Value.Children.Add(compat.Value, 2, 1);

                AbsoluteLayout.SetLayoutBounds(blurCachedImage.Value, new Rectangle(.5, 0, 1, 1));
                AbsoluteLayout.SetLayoutFlags(blurCachedImage.Value, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(FrameCover.Value, new Rectangle(.5, 0, 0.46, 1));
                AbsoluteLayout.SetLayoutFlags(FrameCover.Value, AbsoluteLayoutFlags.All);

                FrameUnderImages.Value.Content = gridInsideFrame.Value;

                absoluteLayout.Value.Children.Add(blurCachedImage.Value);
                absoluteLayout.Value.Children.Add(FrameCover.Value);
                CompressedLayout.SetIsHeadless(absoluteLayout.Value, true);

                panelContainer.Value.Children.Add(FrameUnderImages.Value);

                SubContainer.Value.Children.Add(absoluteLayout.Value);
                CompressedLayout.SetIsHeadless(SubContainer.Value, true);
                Container.Value.Children.Add(SubContainer.Value);
                Container.Value.Children.Add(panelContainer.Value);

                Children.Add(Container.Value);

                tap = new TapGestureRecognizer();

                tap.Tapped += AddToFavListTap;

                compat.Value.GestureRecognizers.Add(tap);

                Result = result;

                var OnTapNavigationGesture = new TapGestureRecognizer();
                OnTapNavigationGesture.Tapped += OnTapNavigationGesture_Tapped;

                GestureRecognizers.Add(OnTapNavigationGesture);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.InnerException);
            }
        }

        private async void OnTapNavigationGesture_Tapped(object sender, EventArgs e)
        {
            ResultSingleton.SetInstance(Result);
            await Shell.Current.GoToAsync("/MovieDetails", true);
        }

        private async void AddToFavListTap(object sender, EventArgs e)
        {
            await pin2FavList.Value.ScaleTo(1.50, 500, Easing.BounceOut);

            IMovieService movieService = Locator.Current.GetService<IMovieService>();

            if (sender != null)
            {
                try
                {
                    await movieService.AddMovieToFavoritesList(Result);

                    await pin2FavList.Value.ScaleTo(1, 500, Easing.BounceIn);
                }
                catch (Exception e15)
                {
                    Debug.WriteLine("Error: " + e15.InnerException);
                }
            }
        }
    }
}
