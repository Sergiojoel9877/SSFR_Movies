using Splat;
using SSFR_Movies.Services.Abstract;
using SSFR_Movies.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            //On<Xamarin.Forms.PlatformConfiguration.Android>().SetBarItemColor(Color.FromHex("#0088FF"));

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(true);

            //On<Xamarin.Forms.PlatformConfiguration.Android>().SetBarSelectedItemColor(Color.White);

            On<Xamarin.Forms.PlatformConfiguration.Android>().EnableSmoothScroll();

        }

        protected override bool OnBackButtonPressed()
        {
            var c = DependencyService.Get<ICloseBackPress>();

            if (c != null)
            {
                c.Close();
                base.OnBackButtonPressed();
                Locator.Current.GetService<FavoriteMoviesPageViewModel>().Dispose();
            }

            return true;
        }
    }
}