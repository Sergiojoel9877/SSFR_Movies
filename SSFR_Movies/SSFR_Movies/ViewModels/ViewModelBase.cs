using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SSFR_Movies.ViewModels
{
    /// <summary>
    /// ViewModelBae: to make ease the implementation of INotifyPorpertyChanged in the properties of the ViewModels
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string name = null)
        {
            if(Equals(storage, value))
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
