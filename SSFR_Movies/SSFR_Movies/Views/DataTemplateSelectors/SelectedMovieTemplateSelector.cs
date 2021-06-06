using System;
using SSFR_Movies.Helpers;
using SSFR_Movies.Models;
using Xamarin.Forms;

namespace SSFR_Movies.Views.DataTemplateSelectors
{
    public class SelectedMovieTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new MovieDataTemplate((Result)item);
    }

    public class MovieDataTemplate : DataTemplate
    {
        public MovieDataTemplate(Result result) : base(() => CreateDataTemplate(result))
        {
        }

        public static View CreateDataTemplate(Result result) => new CustomViewCell(result);
    }
}
