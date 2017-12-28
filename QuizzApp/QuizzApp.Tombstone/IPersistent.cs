namespace QuizzApp.Tombstone
{
    public interface IPersistent
    {
        /// <summary>
        /// Occurs after a class has been instantiated for the first time
        /// </summary>
        void OnLaunched();

        /// <summary>
        /// Occurs after an instance has been rehydrated after an activation
        /// </summary>
        void OnActivated(bool preserved);

        /// <summary>
        /// Occurs before an instance is dehydrated the tombstoned
        /// </summary>
        void OnDeactivated();

        /// <summary>
        /// Occurs when the application is closing
        /// </summary>
        void OnClosing();
    }
}
