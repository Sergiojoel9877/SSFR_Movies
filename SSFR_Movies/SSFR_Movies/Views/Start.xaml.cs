using SSFR_Movies.Services;
using System;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    public partial class Start : ContentPage
    {
        public Start()
        {
            InitializeComponent();

            Task.Run(async () =>
            {
                await FireContainerCommand.ExecuteAsync();
            });
        }

        readonly AsyncCommand fireContainerCommand;
        AsyncCommand FireContainerCommand
        {
            get => fireContainerCommand ?? (new AsyncCommand(async () =>
            {
                await Task.Yield();
      
                new Lazy<ContainerInitializer>(() => new ContainerInitializer()).Value.Initialize();

                await Device.InvokeOnMainThreadAsync(async ()=>
                {
                    ActIndicator.IsVisible = false;

                    ActIndicator.IsRunning = false;

                    Stack.IsVisible = true;

                    await ProBar.ProgressTo(.5, 200, Easing.Linear);
                });

                await Task.WhenAll(Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Task.Yield();

                    Stack.IsVisible = false;

                    Application.Current.MainPage = new Lazy<AppShell>(() => new AppShell()).Value;

                }), ProBar.ProgressTo(100, 200, Easing.Linear));
            }));
        }
    }
}