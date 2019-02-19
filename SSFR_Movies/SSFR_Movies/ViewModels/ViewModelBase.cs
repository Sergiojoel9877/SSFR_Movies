using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms.Internals;

namespace SSFR_Movies.ViewModels
{
    /// <summary>
    /// ViewModelBae: to make ease the implementation of INotifyPorpertyChanged in the properties of the ViewModels
    /// </summary>
    [Preserve(AllMembers = true)]
    public class ViewModelBase : ReactiveObject 
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string name = null)
        {
            if (Equals(storage, value))
                return true;

            storage = value;

            OnPropertyChanged(name);
            return true;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
