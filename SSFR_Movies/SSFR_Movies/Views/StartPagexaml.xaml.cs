using MonkeyCache.FileStore;
using SSFR_Movies.Services;
using SSFR_Movies.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartPagexaml : ContentPage
    {
        public StartPagexaml()
        {
            InitializeComponent();

            FireContainerCommand.Execute(null);
        }

        Command fireContainerCommand;
        Command FireContainerCommand
        {
            get => fireContainerCommand ?? (new Command(async ()=>
            {
                await Task.Delay(2200);

                ActIndicator.IsVisible = false;

                ActIndicator.IsRunning = false;

                Stack.IsVisible = true;

                await ProBar.ProgressTo(.5, 200, Easing.Linear);

                var cont = new ContainerInitializer();
                cont.Initialize();

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