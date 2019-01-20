using CommonServiceLocator;
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
                await Task.Delay(3000);

                ActIndicator.IsVisible = false;

                ActIndicator.IsRunning = false;

                Stack.IsVisible = true;

                await ProBar.ProgressTo(.5, 200, Easing.Linear);

                await Task.Factory.StartNew(()=>
                {
                    ContainerInitializer.Initialize();

                   
                   
                },TaskCreationOptions.RunContinuationsAsynchronously);
                
                var mainPage = new Lazy<MainPage>(()=> new MainPage
                {
                    BarBackgroundColor = Color.FromHex("#272B2E")
                });

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await ProBar.ProgressTo(100, 200, Easing.Linear);

                    Stack.IsVisible = false;

                    await Navigation.PushAsync(mainPage.Value);
                });

            }));
        }
    }
}