using System;

namespace QuizzApp.Tombstone
{
    public enum PersistMode
    {
        None,
        TombstonedOnly,
        ClosedOnly,
        Both
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class Persist : Attribute
    {
        public readonly PersistMode PersistMode = PersistMode.TombstonedOnly;

        /// <summary>
        /// Default is true
        /// </summary> 
        public Persist()
        {

        }

        public Persist(PersistMode mode)
        {
            this.PersistMode = mode;
        }
    }
}
