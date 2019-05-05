
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.ResourceDictionaries
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GenResourceDictionary : ResourceDictionary
    {
        [Preserve]
        public GenResourceDictionary()
        {
            InitializeComponent();
        }
    }
}