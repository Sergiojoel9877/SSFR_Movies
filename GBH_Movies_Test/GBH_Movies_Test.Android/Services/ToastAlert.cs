using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GBH_Movies_Test.Services;

[assembly: Xamarin.Forms.Dependency(typeof(GBH_Movies_Test.Droid.ToastAlert))]
namespace GBH_Movies_Test.Droid
{
    public class ToastAlert : IToast
    {
        public void LongAlert(string msg)
        {
            Toast.MakeText(Application.Context, msg, ToastLength.Long).Show();
        }

        public void ShortAlert(string msg)
        {
            Toast.MakeText(Application.Context, msg, ToastLength.Short).Show();

        }
    }
}