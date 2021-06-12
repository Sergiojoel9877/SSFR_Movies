using System;
using System.Diagnostics;
using FFImageLoading;
using FFImageLoading.Forms;
using Splat;
using SSFR_Movies.Models;
using SSFR_Movies.ViewModels;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace SSFR_Movies.Helpers
{
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public class CustomViewCell : FlexLayout
    {
        #region Controls
        private readonly Lazy<AbsoluteLayout> absoluteLayout = null;
        private readonly Lazy<Frame> FrameUnderImages = null;
        private readonly Lazy<Grid> gridInsideFrame = null;
        private readonly Lazy<Frame> FrameCover = null;
        private readonly Lazy<CachedImage> cachedImage = null;
        private readonly Lazy<Image> pin2FavList = null;
        public Lazy<Label> title = null;
        private readonly Lazy<StackLayout> Container = null;
        private readonly Lazy<StackLayout> SubContainer = null;
        private readonly Lazy<StackLayout> panelContainer = null;
        private readonly Lazy<Label> releaseDate = null;

        Result Result { get; set; }
        #endregion

        public CustomViewCell(Result result)
        {
            try
            {
                Result = result;

                CompressedLayout.SetIsHeadless(this, true);
                HeightRequest = 350;
                Direction = FlexDirection.Column;
                Margin = 16;
                AlignContent = FlexAlignContent.Center;

                Container = new Lazy<StackLayout>(() => new StackLayout()
                {
                    Margin = 8
                });
                CompressedLayout.SetIsHeadless(Container.Value, true);

                SubContainer = new Lazy<StackLayout>(() => new StackLayout()
                {
                    Margin = new Thickness(0, 0, 0, 0),
                    BackgroundColor = Color.Black
                });

                absoluteLayout = new Lazy<AbsoluteLayout>(() => new AbsoluteLayout()
                {
                });
                CompressedLayout.SetIsHeadless(absoluteLayout.Value, true);

                var uriToImgSource = new UriImageSource
                {
                    CacheValidity = TimeSpan.FromDays(15),
                    CachingEnabled = true
                };

                cachedImage = new Lazy<CachedImage>(() => new CachedImage()
                {
                    Aspect = Aspect.Fill,
                    CacheDuration = TimeSpan.MaxValue,
                    LoadingPriority = FFImageLoading.Work.LoadingPriority.Highest,
                    LoadingPlaceholder = "Loading.png",
                    RetryCount = 2,
                    RetryDelay = 15,
                    DownsampleToViewSize = true,
                    Source = new UriImageSource
                    {
                        CacheValidity = TimeSpan.MaxValue,
                        CachingEnabled = true,
                        Uri = new Uri(string.Concat("https://image.tmdb.org/t/p/w370_and_h556_bestv2", Result.PosterPath))
                    }
                });

                FrameCover = new Lazy<Frame>(() => new Frame()
                {
                    IsClippedToBounds = true,
                    Margin = new Thickness(0, 0, 0, 0),
                    HeightRequest = 556,
                    WidthRequest = 370,
                    HasShadow = true,
                    BorderColor = Color.FromHex("#00000000"),
                    Padding = new Thickness(0, 0, 0, 0),
                    BackgroundColor = Color.FromHex("#00000000"),
                    CornerRadius = 15
                });

                panelContainer = new Lazy<StackLayout>(() => new StackLayout()
                {
                });
                CompressedLayout.SetIsHeadless(panelContainer.Value, true);

                FrameUnderImages = new Lazy<Frame>(() => new Frame()
                {
                    IsClippedToBounds = true,
                    HeightRequest = 120,
                    BackgroundColor = Color.Black,
                    CornerRadius = 5,
                    HasShadow = true
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
                    LineBreakMode = LineBreakMode.NoWrap,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontFamily = "Arial",
                    FontAttributes = FontAttributes.Bold,
                    Text = Result.Title
                });

                releaseDate = new Lazy<Label>(() => new Label()
                {
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    TextColor = Color.FromHex("#65FFFFFF"),
                    LineBreakMode = LineBreakMode.NoWrap,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontFamily = "Arial",
                    FontAttributes = FontAttributes.Bold,
                    Text = Result.ReleaseDate
                });

                pin2FavList = new Lazy<Image>(() => new Image()
                {
                    HeightRequest = 40,
                    WidthRequest = 40
                });
                pin2FavList.Value.SetBinding(Image.SourceProperty, "FavoriteMovie");

                FrameCover.Value.Content = cachedImage.Value;

                gridInsideFrame.Value.Children.Add(title.Value, 0, 0);
                Grid.SetColumnSpan(title.Value, 3);
                gridInsideFrame.Value.Children.Add(releaseDate.Value, 0, 1);
                gridInsideFrame.Value.Children.Add(pin2FavList.Value, 2, 1);

                AbsoluteLayout.SetLayoutBounds(FrameCover.Value, new Rectangle(.5, 1, 1, 1));
                AbsoluteLayout.SetLayoutFlags(FrameCover.Value, AbsoluteLayoutFlags.All);

                FrameUnderImages.Value.Content = gridInsideFrame.Value;

                absoluteLayout.Value.Children.Add(FrameCover.Value);
                
                panelContainer.Value.Children.Add(FrameUnderImages.Value);

                SubContainer.Value.Children.Add(absoluteLayout.Value);
                CompressedLayout.SetIsHeadless(SubContainer.Value, true);

                Container.Value.Children.Add(SubContainer.Value);
                Container.Value.Children.Add(panelContainer.Value);

                Children.Add(Container.Value);
                
                var VmInstance = Locator.Current.GetService<AllMoviesPageViewModel>();

                pin2FavList.Value.TapGesture(t =>
                {
                    t.Command = VmInstance.AddToFavListCommand;
                    t.CommandParameter = Result;

                }).TapGesture(r =>
                {
                    r.Command = new AsyncCommand(async () =>
                    {
                        await pin2FavList.Value.ScaleTo(1.50, 500, Easing.BounceOut);
                        await pin2FavList.Value.ScaleTo(1, 500, Easing.BounceIn);
                    });
                });

                this.TapGesture(t =>
                {
                    t.Command = VmInstance.NavToDetailsPage;
                    t.CommandParameter = Result;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.InnerException);
            }
        }
    }
}
