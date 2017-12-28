using System;
using System.IO;
using System.Linq;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace QuizzApp.Tombstone
{
    public class PersistenceStorage
    {

        

        static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        };

        
        /// <summary>
        /// Serialises and stores an object
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public static bool StoreObjectToIsolated<T>(string key, T o, bool overwrite)
        {
            bool stored = false;

            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (overwrite || !storage.FileExists(key))
                {

#if WINDOWS_PHONE
                    string value = JsonConvert.SerializeObject(o, Formatting.None, settings);
                    PhoneApplicationService.Current.State[key] = value;
                    stored = true;
#else
                    using (IsolatedStorageFileStream fileStream = storage.OpenFile(key, FileMode.Create))
                    {

                        string value = JsonConvert.SerializeObject(o, Formatting.None, settings);
                        using (StreamWriter writer = new StreamWriter(fileStream))
                        {
                            try
                            {
                                writer.Write(value);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
#endif

                }
            }

            return stored;
        }

        /// <summary>
        /// Deserialises and retrieves
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetObjectFromIsolated<T>(string key)
        {
            T o = default(T);

#if WINDOWS_PHONE

            if (PhoneApplicationService.Current.State.Keys.Contains(key))
            {
                string data = (string)PhoneApplicationService.Current.State[key];
                o = (T)JsonConvert.DeserializeObject(data, typeof(T), settings);                
            }
#else
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(key))
                {
                    using (IsolatedStorageFileStream fileStream = storage.OpenFile(key, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            o = (T) JsonConvert.DeserializeObject(reader.ReadToEnd(), typeof(T), settings);
                            
                        }
                        //XmlSerializer xs = new XmlSerializer(typeof(T), KnownTypes);
                        //o = (T)xs.Deserialize(fileStream);
                    }
                }
            }
#endif
            return o;
        }

        /// <summary>
        /// Removes an object
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveObjectFromIsolated(string key)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(key))
                {
                    storage.DeleteFile(key);
                }
            }
        }
    }
}
