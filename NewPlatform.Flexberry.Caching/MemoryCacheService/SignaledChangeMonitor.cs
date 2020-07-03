namespace NewPlatform.Flexberry.Caching
{
    using System;
    using System.Globalization;
    using System.Runtime.Caching;

    /// <summary>
    /// Class contains event data for <see cref="SignaledChangeMonitor"/>.
    /// </summary>
    internal class SignaledChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Gets name of signal that raised event.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets name of target cache for signal that raised event.
        /// </summary>
        public string CacheName { get; }

        /// <summary>
        /// Initializes a new instance of the <c>SignaledChangeEventArgs</c> class.
        /// </summary>
        /// <param name="cacheName">Name of target cache for signal that raised event.</param>
        /// <param name="name">Name of signal that raised event.</param>
        public SignaledChangeEventArgs(string cacheName, string name = null)
        {
            CacheName = cacheName;
            Name = name;
        }
    }

    /// <summary>
    /// Cache change monitor that allows an app to fire a change notification
    /// to all associated cache items.
    /// </summary>
    internal class SignaledChangeMonitor : ChangeMonitor
    {
        // Shared across all SignaledChangeMonitors in the AppDomain.
        private static event EventHandler<SignaledChangeEventArgs> Signaled;

        private readonly string _signalName;
        private readonly string _cacheName;

        /// <summary>
        /// Gets a value that represents the <c>SignaledChangeMonitor</c> class instance.
        /// </summary>
        public override string UniqueId { get; } = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

        /// <summary>
        /// Optional handler for signal raising.
        /// </summary>
        public event EventHandler<SignaledChangeEventArgs> SignalHandler;

        /// <summary>
        /// Initializes a new instance of the <c>SignaledChangeMonitor</c> class.
        /// </summary>
        /// <param name="cacheName">Name of cache which is added item with <c>SignaledChangeMonitor</c>.</param>
        /// <param name="signalName">Name of signal for tracking with monitor. Default value is <c>null</c>.</param>
        /// <param name="signalHandler">Event handler for raised signal. Default value is <c>null</c>.</param>
        public SignaledChangeMonitor(string cacheName, string signalName = null, EventHandler<SignaledChangeEventArgs> signalHandler = null)
        {
            _cacheName = cacheName;
            _signalName = signalName;
            SignalHandler = signalHandler;

            // Register instance with the shared event.
            SignaledChangeMonitor.Signaled += OnSignalRaised;
            base.InitializationComplete();
        }

        /// <summary>
        /// Invokes signal handler with specified name.
        /// </summary>
        /// <param name="cacheName">Name of target cache for signal that raised event.</param>
        /// <param name="name">Name of raised signal. Default value is <c>null</c>.</param>
        public static void Signal(string cacheName, string name = null)
        {
            // Raise shared event to notify all subscribers.
            Signaled?.Invoke(null, new SignaledChangeEventArgs(cacheName, name));
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            SignaledChangeMonitor.Signaled -= OnSignalRaised;
        }

        private void OnSignalRaised(object sender, SignaledChangeEventArgs e)
        {
            var cacheNamesAreEqual = string.Compare(e.CacheName, _cacheName, StringComparison.OrdinalIgnoreCase) == 0;
            var signalNamesAreEqual = string.Compare(e.Name, _signalName, StringComparison.OrdinalIgnoreCase) == 0;
            if (cacheNamesAreEqual && (string.IsNullOrWhiteSpace(e.Name) || signalNamesAreEqual))
            {
                SignalHandler?.Invoke(this, e);

                // Cache objects are obligated to remove entry upon change notification.
                base.OnChanged(null);
            }
        }
    }
}
