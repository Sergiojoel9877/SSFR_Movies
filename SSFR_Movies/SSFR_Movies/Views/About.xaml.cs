
using AsyncAwaitBestPractices.MVVM;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    [Preserve(AllMembers = true)]
   
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

                Command = new AsyncCommand(async () =>
                {
                    await Shell.Current.GoToAsync("/Search", false);
                })
            };

            ToolbarItems.Add(searchToolbarItem);
        }
    }
}