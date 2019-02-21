﻿using FFImageLoading.Transformations;
using Realms;
using SSFR_Movies.Converters;
using SSFR_Movies.Models;
using SSFR_Movies.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace SSFR_Movies.Helpers
{
    [Preserve(AllMembers = true)]
    public class CustomViewCell : FlexLayout
    {
        #region Controls
        private Lazy<Image> blurCachedImage = null;
        private Lazy<Image> cachedImage = null;
        private Lazy<StackLayout> Container = null;
        private Lazy<StackLayout> SubContainer = null;
        private Lazy<AbsoluteLayout> absoluteLayout = null;
        private Lazy<StackLayout> panelContainer = null;
        private Lazy<Frame> FrameUnderImages = null;
        private Lazy<Grid> gridInsideFrame = null;
        private Lazy<Label> releaseDate = null;
        public Lazy<Label> title = null;
        private Lazy<Image> pin2FavList = null;
        private Lazy<StackLayout> compat = null;
        private TapGestureRecognizer tap = null;
        private TapGestureRecognizer imageTapped = null;

        #endregion

        public CustomViewCell()
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

            SubContainer = new Lazy<StackLayout>(()=> new StackLayout()
            {
                Margin = new Thickness(16, 0, 16, 0),
                HeightRequest = 280,
                BackgroundColor = Color.FromHex("#44312D2D"),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            });

            absoluteLayout = new Lazy<AbsoluteLayout>(()=> new AbsoluteLayout()
            {
                VerticalOptions = LayoutOptions.FillAndExpand
            });

            List<FFImageLoading.Work.ITransformation> Blur = new List<FFImageLoading.Work.ITransformation>
            {
                new BlurredTransformation(15)
            };

            blurCachedImage = new Lazy<Image>(() => new Image()
            {
                HeightRequest = 330,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Scale = 3,
                VerticalOptions = LayoutOptions.FillAndExpand,
                WidthRequest = 330
            });
            blurCachedImage.Value.SetBinding(Image.SourceProperty, new Binding("BackdropPath", BindingMode.Default, new BackgroundImageUrlConverter()));

            cachedImage = new Lazy<Image>(() => new Image()
            {
                HeightRequest = 280,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                WidthRequest = 280
            });
            cachedImage.Value.SetBinding(Image.SourceProperty, new Binding("PosterPath", BindingMode.Default, new PosterImageUrlConverter()));
            
            panelContainer = new Lazy<StackLayout>(()=> new StackLayout()
            {
                HeightRequest = 125,
                HorizontalOptions = LayoutOptions.Center,
            });

            FrameUnderImages = new Lazy<Frame>(()=> new Frame()
            {
                BackgroundColor = Color.FromHex("#44312D2D"),
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.Center
            });
            FrameUnderImages.Value.On<Xamarin.Forms.PlatformConfiguration.Android>().SetElevation(6f);

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

            gridInsideFrame = new Lazy<Grid>(()=> new Grid()
            {
                ColumnDefinitions = columnDefinitions,
                RowDefinitions = rowDefinitions
            });

            title = new Lazy<Label>(()=> new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.White,
                LineBreakMode = LineBreakMode.TailTruncation,
                Margin = new Thickness(16, 0, 0, 0),
                FontFamily = "Arial",
                FontAttributes = FontAttributes.Bold
            });

            title.Value.SetBinding(Label.TextProperty, "Title");
            
            releaseDate = new Lazy<Label>(()=> new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = Color.FromHex("#65FFFFFF"),
                LineBreakMode = LineBreakMode.NoWrap,
                Margin = new Thickness(16, 0, 0, 0),
                FontFamily = "Arial",
                FontAttributes = FontAttributes.Bold
            });
            releaseDate.Value.SetBinding(Label.TextProperty, "ReleaseDate");

            compat = new Lazy<StackLayout>(()=> new StackLayout()
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

            compat.Value.Children.Add(pin2FavList.Value);
            
            gridInsideFrame.Value.Children.Add(title.Value, 0, 0);
            Grid.SetColumnSpan(title.Value, 3);
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

            imageTapped = new TapGestureRecognizer();

            tap.Tapped += AddToFavListTap;

            imageTapped.Tapped += PosterTapped;

            absoluteLayout.Value.GestureRecognizers.Add(imageTapped);

            compat.Value.GestureRecognizers.Add(tap);
        }

        private void PosterTapped(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "Hide", true);
        }

        private async void AddToFavListTap(object sender, EventArgs e)
        {
            await Task.Yield();

            await pin2FavList.Value.ScaleTo(1.50, 500, Easing.BounceOut);

            if (sender != null)
            {

                var movie = BindingContext as Result;

                //Verify if internet connection is available
                if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
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

                    var movieExists = realm.Find<Result>(movie.Id);

                    if (movieExists != null && movieExists.FavoriteMovie == "Star.png")
                    {
                        DependencyService.Get<IToast>().LongAlert("Oh no It looks like " + movie.Title + " already exits in your favorite list!");

                        await pin2FavList.Value.ScaleTo(1, 500, Easing.BounceIn);
                        
                        return;
                    }
                    
                    realm.Write(() =>
                    {
                        movie.FavoriteMovie = "Star.png";

                        realm.Add(movie, true);
                    });

                    MessagingCenter.Send(this, "Refresh", true);

                    DependencyService.Get<IToast>().LongAlert("Added Successfully, The movie " + movie.Title + " was added to your favorite list!");
                    
                    await pin2FavList.Value.ScaleTo(1, 500, Easing.BounceIn);

                    await SpeakNow("Added Successfully").ConfigureAwait(false);
                    
                }
                catch (Exception e15)
                {
                    Debug.WriteLine("Error: " + e15.InnerException);
                }
            }
        }

        public async Task SpeakNow(string msg)
        {
            var settings = new SpeechOptions
            {
                Volume = 1f,
                Pitch = 1.0f
            };

            await TextToSpeech.SpeakAsync(msg, settings);
        }
    }
}
