using System;
using System.Collections.Generic;
using System.Text;


using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Xamarin.Forms.Internals;

namespace SSFR_Movies.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    [Preserve(AllMembers = true)]
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string SettingsKey = "settings_key";
        private static readonly string SettingsDefault = string.Empty;

        #endregion


        public static string GeneralSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(SettingsKey, value);
            }
        }

        public static int NextPage
        {
            get
            {
                return AppSettings.GetValueOrDefault("NextPage", 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue("NextPage", value);
            }
        }

        public static bool UpdateList
        {
            get => AppSettings.GetValueOrDefault("UpdateList", false);
            set => AppSettings.AddOrUpdateValue("UpdateList", value);
        }

        public static int NextPageForGenre
        {
            get
            {
                return AppSettings.GetValueOrDefault("NextPageForGenre", 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue("NextPageForGenre", value);
            }
        }

    }
}