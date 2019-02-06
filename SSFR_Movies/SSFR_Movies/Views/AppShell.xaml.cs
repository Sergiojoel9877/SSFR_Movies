using SSFR_Movies.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static SSFR_Movies.Views.SearchPage;

namespace SSFR_Movies.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {

        ToolbarItem searchToolbarItem = null;

        public AppShell()
        {
            InitializeComponent();

            SetRoutes();
        }

        void SetRoutes()
        {
            Routing.RegisterRoute("moviedetailspage", typeof(MovieDetailsPage));
            Routing.RegisterRoute("searchpage", typeof(SearchPage));
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