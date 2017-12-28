using SimpleMvvmToolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace QuizzApp.Core.Helpers
{

    public class AppSettingsProvider
    {
        public AppSettings AppSettings { get { return AppSettings.Instance; } }
    }


    public class AppSettings : ModelBase<AppSettings>
    {
        #region singleton pattern

        private static readonly AppSettings instance = new AppSettings();
        
        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        private AppSettings()
        {
            // Get the settings for this application.
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        public static AppSettings Instance
           {
              get 
              {
                 return instance; 
              }
           }

        #endregion


        // The key names of our settings
        const string SoundOnOffKeyName = "SoundOnOff";
        const string SoundVolumeKeyName = "SoundVolume";
        const string FcbAccessTokenKeyName = "FcbAccessToken";
        const string FcbUserIdKeyName = "FcbUserId";
        const string HasHelpPageAlreadyBeSeenKeyName = "HasHelpPageAlreadyBeSeen";
        const string IsQuizzAppFirstAppLaunchKeyName = "IsQuizzAppFirstAppLaunch";
        const string LanguageOnAppLaunchBckKeyName = "LanguageOnAppLaunchBck"; 
        const string AppCurrentLanguageKeyName = "AppCurrentLanguage";

        // The default value of our settings
        const bool SoundOnOffDefault = true;
        const double SoundVolumeDefault = 0.8d;
        const bool HasHelpPageAlreadyBeSeenDefault = false;
        const bool IsFirstAppLaunchDefault = true;
        const string LanguageOnAppLaunchBckDefaultString = "en-EN";
        const string AppCurrentLanguageDefaultString = "en-EN";
        
        // Our settings
        IsolatedStorageSettings settings;

        private object _locker = new object();

        public double SoundVolumeSetting
        {
            get
            {
                return GetValueOrDefault<double>(SoundVolumeKeyName, SoundVolumeDefault);
            }
            private set
            {
                if (AddOrUpdateValue(SoundVolumeKeyName, value))
                {
                    Save();
                    NotifyPropertyChanged(m => m.SoundVolumeSetting);
                }
            }
        }
        
        public bool SoundOnOffSetting
        {
            get
            {
                return GetValueOrDefault<bool>(SoundOnOffKeyName, SoundOnOffDefault);
            }
            set
            {
                if (AddOrUpdateValue(SoundOnOffKeyName, value))
                {
                    Save();
                    NotifyPropertyChanged(m => m.SoundOnOffSetting);
                    if (value == true)
                        this.SoundVolumeSetting = SoundVolumeDefault;
                    else
                        this.SoundVolumeSetting = 0;                   
                }
            }
        }


        public string FcbAccessToken
        {
            get
            {
                return GetValueOrDefault<string>(FcbAccessTokenKeyName, string.Empty);
            }
            set
            {
                if (AddOrUpdateValue(FcbAccessTokenKeyName, value))
                {
                    Save();
                    NotifyPropertyChanged(m => m.FcbAccessToken);
                }
            }
        }


        public string FcbUserId
        {
            get
            {
                return GetValueOrDefault<string>(FcbUserIdKeyName, string.Empty);
            }
            set
            {
                if (AddOrUpdateValue(FcbUserIdKeyName, value))
                {
                    Save();
                    NotifyPropertyChanged(m => m.FcbUserId);
                }
            }
        }


        public bool HasHelpPageAlreadyBeSeen
        {
            get
            {
                return GetValueOrDefault<bool>(HasHelpPageAlreadyBeSeenKeyName, HasHelpPageAlreadyBeSeenDefault);                
            }
            set
            {
                if (AddOrUpdateValue(HasHelpPageAlreadyBeSeenKeyName, value))
                {
                    Save();
                    NotifyPropertyChanged(m => m.HasHelpPageAlreadyBeSeen);
                }
            }
        }


        public bool IsQuizzAppFirstAppLaunch
        {
            get
            {
                return GetValueOrDefault<bool>(IsQuizzAppFirstAppLaunchKeyName, IsFirstAppLaunchDefault);
            }
            set
            {
                if (AddOrUpdateValue(IsQuizzAppFirstAppLaunchKeyName, value))
                {
                    Save();
                    NotifyPropertyChanged(m => m.IsQuizzAppFirstAppLaunch);
                }
            }
        }

        //public bool IsQuizzAppFirstAppLaunchForLangId(string langId)
        //{
        //    string cultureId = IsQuizzAppFirstAppLaunchKeyName + "_" + langId;
        //    if (!this.ContainsKey(cultureId))
        //    {
        //        this.AddOrUpdateValue(cultureId, true);
        //    }
        //    return this.GetValueOrDefault<bool>(cultureId, true);
        //}


        //public void SetIsQuizzAppFirstAppLaunchForLangId(string langId, bool value)
        //{
        //    string cultureId = IsQuizzAppFirstAppLaunchKeyName + "_" + langId;
        //    this.AddOrUpdateValue(cultureId, value);
        //}

        public CultureInfo AppCurrentLanguage
        {
            get
            {
                return new CultureInfo(GetValueOrDefault<string>(AppCurrentLanguageKeyName, AppCurrentLanguageDefaultString));
            }
            set
            {
                string stringVal = AppCurrentLanguageDefaultString;
                if (value != null)
                     stringVal = value.ToString();
                if (AddOrUpdateValue(AppCurrentLanguageKeyName, stringVal))
                {
                    Save();
                    NotifyPropertyChanged(m => m.AppCurrentLanguage);
                }
            }
        }

        public bool HasAppLanguageSetted
        {
            get
            {
                return settings.Contains(AppCurrentLanguageKeyName);
            }
        }



        public CultureInfo LanguageOnAppLaunchBck
        {
            get
            {
                return new CultureInfo(GetValueOrDefault<string>(LanguageOnAppLaunchBckKeyName, LanguageOnAppLaunchBckDefaultString));
            }
            set
            {
                string stringVal = LanguageOnAppLaunchBckDefaultString;
                if (value != null)
                    stringVal = value.ToString();
                if (AddOrUpdateValue(LanguageOnAppLaunchBckKeyName, stringVal))
                {
                    Save();
                    NotifyPropertyChanged(m => m.LanguageOnAppLaunchBck);
                }
            }
        }


        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
                        
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            lock (_locker)
            {
                // If the key exists, retrieve the value.
                if (settings.Contains(Key))
                {
                    try
                    {
                        value = (T)settings[Key];
                    }
                    catch (InvalidCastException)
                    {
                        settings[Key] = defaultValue;
                        Save();
                        return defaultValue;
                    }
                }
                // Otherwise, use the default value.
                else
                {
                    value = defaultValue;
                }
            }

            return value;
        }

               
        public bool ContainsKey(string key)
        {
            lock (_locker)
            {
                // If the key exists, retrieve the value.
                return settings.Contains(key);
            }
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            lock (_locker)
            {
                settings.Save();
            }
        }

                
    }
}
