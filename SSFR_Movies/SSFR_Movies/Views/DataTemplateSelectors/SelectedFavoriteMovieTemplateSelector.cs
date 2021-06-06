using System;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using Xamarin.Forms;

namespace SSFR_Movies.Views.DataTemplateSelectors
{
    public class SelectedFavoriteMovieTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new FavoriteMovieDataTemplate((Result)item);
    }

    public class FavoriteMovieDataTemplate : DataTemplate
    {
        public FavoriteMovieDataTemplate(Result result) : base(() => CreateDataTemplate(result))
        {
        }

        public static View CreateDataTemplate(Result result) => new CustomViewCellFavPage(result);
    }
}
