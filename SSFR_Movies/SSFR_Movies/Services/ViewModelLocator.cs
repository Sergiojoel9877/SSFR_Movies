using System;
using System.Collections.Generic;
using System.Text;
using CommonServiceLocator;
using SSFR_Movies.ViewModels;
using Xamarin.Forms.Internals;

namespace SSFR_Movies.Services
{
    /// <summary>
    /// To take the instance registered of each ViewModel stored in the IoC ServiceLocator 
    /// </summary>
  
    public class ViewModelLocator
    {
      
        public ViewModelLocator()
        {

        }

        public AllMoviesPageViewModel AllMoviesPageViewModel
        {
            get => ServiceLocator.Current.GetInstance<AllMoviesPageViewModel>();
        }

        public FavoriteMoviesPageViewModel FavoriteMoviesPageViewModel
        {
            get => ServiceLocator.Current.GetInstance<FavoriteMoviesPageViewModel>();
        }
    }
}
