using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace QuizzApp.Tombstone
{
    public enum ApplicationMode
    {
        Unknown,
        Launching,
        Activated,
        Deactivated,
        Closing
    }

    public sealed class PersistenceManager
    {
        private readonly Dictionary<string, object> _persistentObjects = new Dictionary<string, object>();
        private List<PersistenceProperties> _rehydratedProperties = null;
        private readonly object _synchLock = new object();
        public ApplicationMode ApplicationMode = ApplicationMode.Unknown;
        private const string _persistenceProperties = "PersistenceProperties";
        //private bool _appReady = false;
        private bool _wasPreserved = false;

        internal List<PersistenceProperties> RehydratedProperties
        {
            get { return this._rehydratedProperties; }
        }

        // Singleton
        public static readonly PersistenceManager _instance = new PersistenceManager();

        public static PersistenceManager Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Internal constructor used for unit testing instances
        /// </summary>
        internal PersistenceManager()
        {

        }

        
        public void OnLaunching()
        {
            this.ApplicationMode = ApplicationMode.Launching;

            //this._appReady = true;

            // Load properties
            this.GetPropertiesFromStorage();

            // Initialise regiestered
            //this.InitialiseRegistered(false);
        }

        public void OnActivated(bool preserved)
        {
            this.ApplicationMode = ApplicationMode.Activated;

            //this._appReady = true;
            this._wasPreserved = preserved;

            // If app instance is not preserved load names so we know whether instances need rehydrating or not
            if (!preserved)
            {
                this.GetPropertiesFromStorage();                
            }

            // Initialise regiestered
            //this.InitialiseRegistered(preserved);
        }

        public void OnDeactivated()
        {
            this.ApplicationMode = ApplicationMode.Deactivated;

            // Dehydrate all objects
            DehydrateObjects();
        }

        public void OnClosing()
        {
            this.ApplicationMode = ApplicationMode.Closing;

            // Dehydrate all objects
            DehydrateObjects();
        }

        /// <summary>
        /// Register IPersistent for monitoring
        /// </summary>
        /// <param name="persistent"></param>
        public void Register(string uniqueName, object o)
        {
            
            lock (_synchLock)
            {
                // New instantiation
                if (!_persistentObjects.ContainsKey(uniqueName))
                {
                    _persistentObjects.Add(uniqueName, o);

                    //// Notify object to initialise if the app is ready
                    //if (o is IPersistent && _appReady)
                    //{
                    //    if (ApplicationMode == ApplicationMode.Launching)
                    //    {
                    //        (o as IPersistent).OnLaunched();
                    //    }
                    //    else if (ApplicationMode == ApplicationMode.Activated)
                    //    {
                    //        (o as IPersistent).OnActivated(false);
                    //    }
                    //}
                }
                else // Rehydration
                {
                    _persistentObjects[uniqueName] = o;                    
                    Rehydrate(uniqueName, o);
                }
            }
        }

        public void UnRegister(string uniqueName)
        {
            lock (_synchLock)
            {
                if (_persistentObjects.ContainsKey(uniqueName))
                {
                    _persistentObjects.Remove(uniqueName);
                }
            }
        }


        public void PageNavigated(object viewModel)
        {            
            this.InitialiseRegistered(this._wasPreserved, viewModel);
            this._wasPreserved = false;
        }

        

        private void InitialiseRegistered(bool preserved = false, object objectToInitialize = null)
        {
            if (_persistentObjects != null)
            {
                if (objectToInitialize != null)
                {
                    object o = _persistentObjects.Values.Where(m => m == objectToInitialize).FirstOrDefault();
                    if (o != null && o is IPersistent)
                    {
                        // Notify object to initialise
                        if (o is IPersistent)
                        {
                            if (ApplicationMode == ApplicationMode.Launching)
                            {
                                (o as IPersistent).OnLaunched();
                            }
                            else if (ApplicationMode == ApplicationMode.Activated)
                            {
                                (o as IPersistent).OnActivated(preserved);
                            }
                        }
                    }
                }
                else
                {
                    foreach (object o in _persistentObjects.Values)
                    {
                        // Notify object to initialise
                        if (o is IPersistent)
                        {
                            if (ApplicationMode == ApplicationMode.Launching)
                            {
                                (o as IPersistent).OnLaunched();
                            }
                            else if (ApplicationMode == ApplicationMode.Activated)
                            {
                                (o as IPersistent).OnActivated(preserved);
                            }
                        }
                    }
                }
            }
        }


        private void GetPropertiesFromStorage()
        {
            // Get properties from storage
            _rehydratedProperties = PersistenceStorage.GetObjectFromIsolated<List<PersistenceProperties>>(_persistenceProperties);

            LoadProperties();
        }


        private void LoadProperties()
        {
            lock (_synchLock)
            {
                if (_rehydratedProperties != null)
                {
                    _rehydratedProperties.TrimExcess();

                    // Add object place holders to indicate they need re-hydrating once they are instantiated and registered
                    foreach (string name in _rehydratedProperties.Select(p => p.UniqueName).Distinct())
                    {
                        if (!_persistentObjects.ContainsKey(name))
                        {
                            _persistentObjects.Add(name, null);
                        }
                        else
                        {
                            // Rehydrate
                            Rehydrate(name, _persistentObjects[name]);
                        }
                    }
                }
            }
        }

        private void CloseObjects()
        {
            foreach (object o in _persistentObjects.Values)
            {
                // Notify object to initialise
                if (o is IPersistent)
                    (o as IPersistent).OnClosing();
            }
        }


        private void DehydrateObjects()
        {
            List<PersistenceProperties> dehydrated = new List<PersistenceProperties>();

            // Examine each object
            foreach (KeyValuePair<string, object> obj in _persistentObjects)
            {
                if (obj.Value != null)
                {
                    // Notify object that it is about to be dehydrated
                    if (obj.Value is IPersistent && ApplicationMode == ApplicationMode.Deactivated)
                    {
                        (obj.Value as IPersistent).OnDeactivated();
                    }
                    else if (obj.Value is IPersistent && ApplicationMode == ApplicationMode.Closing)
                    {
                        (obj.Value as IPersistent).OnClosing();
                    }

                    IEnumerable<PropertyInfo> pi = null;

                    pi = obj.Value.GetType().GetProperties();


                    // Examine each property
                    foreach (PropertyInfo p in pi)
                    {
                        // Add any properties with Persist attribute
                        object[] atts = null;

                        atts = p.GetCustomAttributes(typeof(Persist), true);


                        if (atts != null && atts.Length > 0)
                        {
                            PersistMode mode = (atts[0] as Persist).PersistMode;
                            if ((this.ApplicationMode == ApplicationMode.Deactivated &&
                                (mode == PersistMode.TombstonedOnly || mode == PersistMode.Both)) ||
                                (this.ApplicationMode == ApplicationMode.Closing &&
                                (mode == PersistMode.ClosedOnly || mode == PersistMode.Both)))
                            {
                                var tsp = new PersistenceProperties(obj.Key, obj.Value.GetType().Name, p.Name, p.GetValue(obj.Value, null));

                                dehydrated.Add(tsp);
                            }
                        }
                    }
                }
            }

            dehydrated.TrimExcess();


            PersistenceStorage.StoreObjectToIsolated<List<PersistenceProperties>>(_persistenceProperties, dehydrated, true);

        }

        private void Rehydrate(string uniqueName, object o)
        {
            try
            {
                IEnumerable<PropertyInfo> pi = null;

                pi = o.GetType().GetProperties();


                // Examine each property
                foreach (PropertyInfo p in pi)
                {
                    // Add any properties with Persist attribute
                    object[] atts = null;
                    atts = p.GetCustomAttributes(typeof(Persist), true);

                    if (atts != null && atts.Length > 0)
                    {
                        PersistMode mode = (atts[0] as Persist).PersistMode;
                        if (_rehydratedProperties != null 
                            && (this.ApplicationMode == ApplicationMode.Activated &&
                            (mode == PersistMode.TombstonedOnly || mode == PersistMode.Both)) ||
                            (this.ApplicationMode == ApplicationMode.Launching &&
                            (mode == PersistMode.ClosedOnly || mode == PersistMode.Both)))
                        {

                            // Get value
                            PersistenceProperties propertyPersisted = _rehydratedProperties.SingleOrDefault(r => r.UniqueName == uniqueName &&
                            r.ClassName == o.GetType().Name && r.PropertyName == p.Name);

                            if (propertyPersisted != null)
                            {
                                object value = propertyPersisted.Value;                                
                                
                                Type nullableType = Nullable.GetUnderlyingType(p.PropertyType);
                                if (nullableType != null)
                                {
                                    if (nullableType.IsEnum)
                                    {
                                        value = Enum.ToObject(nullableType, value);
                                    }
                                    else
                                    {
                                        value = (value == null) ? null : Convert.ChangeType(value, nullableType, CultureInfo.CurrentCulture);
                                    }
                                }
                                else if (p.PropertyType.IsGenericType)
                                {
                                    value = Convert.ChangeType(value, p.PropertyType.GetGenericTypeDefinition(), CultureInfo.CurrentCulture);
                                }
                                else
                                {
                                    if (p.PropertyType.IsEnum)
                                    {
                                        value = Enum.ToObject(p.PropertyType, value);
                                    }                                   
                                    else
                                    {
                                        value = Convert.ChangeType(value, p.PropertyType, CultureInfo.CurrentCulture);
                                    }
                                }

                                // Assign value
                                p.SetValue(o,  value, null);

                                // remove from _rehydrated properties
                                _rehydratedProperties.Remove(propertyPersisted);
                            }
                        }
                    }
                }

                // Notify object that it has been hydrated
                //if (o is IPersistent)
                //{
                //    if (this.ApplicationMode == ApplicationMode.Activated)
                //    {
                //        (o as IPersistent).OnActivated(false, true);
                //    }
                //    else if (this.ApplicationMode == ApplicationMode.Launching)
                //    {
                //        (o as IPersistent).OnLaunched();
                //    }
                //}
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error setting value on reactivation " + e);
                // If anything has gone wrong notify the object to initialise normally
                //if (o is IPersistent)
                //    (o as IPersistent).OnLaunched();
            }
        }


        
      
    }

    
}
