using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Internals;

namespace SSFR_Movies.ViewModels
{
    /// <summary>
    /// ViewModelBae: to make ease the implementation of INotifyPorpertyChanged in the properties of the ViewModels
    /// </summary>
    [Preserve(AllMembers = true)]
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void Dispose()
        {
        }

        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string name = null)
        {
            if (Equals(storage, value))
            {
                return true;
            }

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
