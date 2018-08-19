using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace GBH_Movies_Test.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        public MainPage ()
        {
            InitializeComponent();

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetBarItemColor(Color.FromHex("#0088FF"));

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(true);

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetBarSelectedItemColor(Color.White);

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetElevation(5.0f);

        }
    }
}