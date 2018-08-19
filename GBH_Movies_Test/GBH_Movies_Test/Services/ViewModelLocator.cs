using System;
using System.Collections.Generic;
using System.Text;
using CommonServiceLocator;
using GBH_Movies_Test.ViewModels;

namespace GBH_Movies_Test.Services
{
    /// <summary>
    /// To take the instance registered of each ViewModel stored in the IoC ServiceLocator 
    /// </summary>
    public class ViewModelLocator
    {
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
