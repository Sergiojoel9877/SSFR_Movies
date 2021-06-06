using System.ComponentModel;
using System.Runtime.CompilerServices;
using SSFR_Movies.Services.Abstract;
using SSFR_Movies.ViewModels;
using Xamarin.Forms;

namespace SSFR_Movies.Views
{

    public partial class AppShell : Shell
    {

        bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled, value);
        }

        public AppShell()
        {
            InitializeComponent();

            //RegisterMessage();
            RegisterRoutes();
        }

        void RegisterRoutes()
        {
            Routing.RegisterRoute("Search", typeof(SearchPage));
            Routing.RegisterRoute("MovieDetails", typeof(MovieDetailsPage));
        }

        private void RegisterMessage()
        {
            MessagingCenter.Subscribe<AllMoviesPageViewModel, bool>(this, "HIDE", (p, e) =>
            {
                Enabled = e;
            });
            MessagingCenter.Subscribe<AllMoviesPageViewModel, bool>(this, "SHOW", (p, e) =>
            {
                Enabled = e;
            });
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

        public event PropertyChangedEventHandler _PropertyChanged;

        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string name = null)
        {
            if (Equals(storage, value))
            {
                return true;
            }

            storage = value;

            OnPropertyChanged(name);
            return true;
        }

        protected override void OnPropertyChanged(string name)
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}