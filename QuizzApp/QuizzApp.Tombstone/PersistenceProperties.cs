using System;

namespace QuizzApp.Tombstone
{
    /// <summary>
    /// This class is used to store object properties marked with the Persist attribute
    /// </summary>
    public class PersistenceProperties
    {
        public string UniqueName { get; set; }
        public string ClassName { get; set; }
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public PersistenceProperties()
        {

        }

        public PersistenceProperties(string uniqueName, string className, string propertyName, object value)
        {
            this.UniqueName = uniqueName;
            this.ClassName = className;
            this.PropertyName = propertyName;
            this.Value = value;
        }
    }
}
