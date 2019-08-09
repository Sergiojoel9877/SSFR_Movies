using SSFR_Movies.Services;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    public partial class Start : ContentPage
    {
        public Start()
        {
            InitializeComponent();

            FireContainerCommand.Execute(null);
        }

        readonly Command fireContainerCommand = null;
        Command FireContainerCommand
        {
            get => fireContainerCommand ?? (new Command(async () =>
            {
                ActIndicator.IsVisible = false;

                ActIndicator.IsRunning = false;

                Stack.IsVisible = true;

                await ProBar.ProgressTo(.5, 200, Easing.Linear);

                new Lazy<ContainerInitializer>(() => new ContainerInitializer()).Value.Initialize();

                var mainPage = new Lazy<AppShell>(() => new AppShell());

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await ProBar.ProgressTo(100, 200, Easing.Linear);

                    Stack.IsVisible = false;

                    App.Current.MainPage = mainPage.Value;
                });

            }));
        }
    }
}