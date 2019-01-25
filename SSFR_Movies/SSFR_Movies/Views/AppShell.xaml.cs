using SSFR_Movies.Services;
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
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        Command fireContainerCommand;
        Command FireContainerCommand
        {
            get => fireContainerCommand ?? (new Command(async () =>
            {
                await Task.Delay(2200);
                
                await Task.Factory.StartNew(() =>
                {
                    ContainerInitializer.Initialize();
                }, TaskCreationOptions.RunContinuationsAsynchronously);

                //var mainPage = new Lazy<MainPage>(()=> new MainPage
                //{
                //    BarBackgroundColor = Color.FromHex("#272B2E")
                //});
                //var mainPage = new Lazy<AppShell>(() => new AppShell());
                var mainPage = new AppShell();

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.Navigation.PushAsync(mainPage);
                });

            }));
        }
        protected override bool OnBackButtonPressed()
        {
            var c = DependencyService.Get<ICloseBackPress>();

            if (c != null)
            {
                c.Close();
                base.OnBackButtonPressed();
            }

            return true;
        }
    }
}