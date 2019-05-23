
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class About : ContentPage
    {
        readonly ToolbarItem searchToolbarItem = null;

        public About()
        {
            InitializeComponent();

            searchToolbarItem = new ToolbarItem()
            {
                Text = "Search",
                IconImageSource = "Search.png",
                Priority = 0,

                Command = new Command(async () =>
                {
                    await Navigation.PushAsync(new SearchPage(), true);
                })
            };

            ToolbarItems.Add(searchToolbarItem);
        }
    }
}