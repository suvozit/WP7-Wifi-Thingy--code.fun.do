using System;
using System.Threading;
using System.Windows;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using PAARC.Shared;

namespace PAARC.WP7.Services
{
    // This class is based on an implementation by Andrea Boschin that can be found in his article on Silverlight Show:
    // http://www.silverlightshow.net/items/Windows-Phone-7-Data-Access-Strategies-Connection-detection-bandwidth.aspx
    // Changes introduced compared to the original implementation are:
    // * Using the phone application service events to initiate network readings
    // * Initializing all values to indeterminate state
    //   => These two changes fix a bug where a certain network type would be reported after deactivation even if that network is not available
    // * Removed a hard-wired call to MessageBox.Show in case of an error
    // * Added an error event and event args class
    // * Removed NetworkErrorEventArgs because they're already defined in the Shared assembly

    public class NetworkWatchdog : IApplicationService
    {
        private object _lockObj = new object();
        private bool? _isNetworkAvailable;
        private NetworkInterfaceType _interfaceType;

        public event EventHandler<NetworkChangedEventArgs> NetworkChanged;
        public event EventHandler<NetworkErrorEventArgs> Error;

        /// <summary>
        /// Gets the current.
        /// </summary>
        public static NetworkWatchdog Current { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is network available.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is network available; otherwise, <c>false</c>.
        /// </value>
        public bool IsNetworkAvailable
        {
            get
            {
                return _isNetworkAvailable.HasValue && _isNetworkAvailable.Value;
            }
            private set
            {
                _isNetworkAvailable = value;
            }
        }

        /// <summary>
        /// Gets the type of the interface.
        /// </summary>
        /// <value>
        /// The type of the interface.
        /// </value>
        public NetworkInterfaceType InterfaceType
        {
            get
            {
                return _interfaceType;
            }
            private set
            {
                _interfaceType = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkWatchdog"/> class.
        /// </summary>
        public NetworkWatchdog()
        {
            if (Current != null)
            {
                throw new Exception("Only a single watchdog is allowed");
            }

            Current = this;

            InitializeToIndeterminate();

            // is null in design time mode
            if (PhoneApplicationService.Current != null)
            {
                PhoneApplicationService.Current.Launching += PhoneApplicationService_Launching;
                PhoneApplicationService.Current.Activated += PhoneApplicationService_Activated;
                PhoneApplicationService.Current.Deactivated += PhoneApplicationService_Deactivated;
                PhoneApplicationService.Current.Closing += PhoneApplicationService_Closing;
            }
        }

        private void InitializeToIndeterminate()
        {
            _isNetworkAvailable = null;
            _interfaceType = NetworkInterfaceType.Unknown;
        }

        private void PhoneApplicationService_Launching(object sender, LaunchingEventArgs e)
        {
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
            InitializeToIndeterminate();
            ThreadPool.QueueUserWorkItem(ReadNetworkInformation, null);
        }

        private void PhoneApplicationService_Activated(object sender, ActivatedEventArgs e)
        {
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
            InitializeToIndeterminate();
            ThreadPool.QueueUserWorkItem(ReadNetworkInformation, null);
        }

        private void PhoneApplicationService_Deactivated(object sender, DeactivatedEventArgs e)
        {
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged -= NetworkChange_NetworkAddressChanged;
        }

        private void PhoneApplicationService_Closing(object sender, ClosingEventArgs e)
        {
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged -= NetworkChange_NetworkAddressChanged;
        }

        /// <summary>
        /// Called by an application in order to initialize the application extension service.
        /// </summary>
        /// <param name="context">Provides information about the application state.</param>
        public void StartService(ApplicationServiceContext context)
        {
        }

        /// <summary>
        /// Called by an application in order to stop the application extension service.
        /// </summary>
        public void StopService()
        {
        }

        /// <summary>
        /// Handles the NetworkAddressChanged event of the NetworkChange control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(ReadNetworkInformation, null);
        }

        /// <summary>
        /// Reads the network information.
        /// </summary>
        /// <param name="state">The state.</param>
        private void ReadNetworkInformation(object state)
        {
            try
            {
                bool isAvailable = NetworkInterface.GetIsNetworkAvailable();
                NetworkInterfaceType interfaceType = NetworkInterface.NetworkInterfaceType;

                bool isAvailableChanged = false;
                bool isTypeChanged = false;

                lock (_lockObj)
                {
                    if (this.IsNetworkAvailable != isAvailable)
                    {
                        this.IsNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
                        isAvailableChanged = true;
                    }

                    if (this.InterfaceType != interfaceType)
                    {
                        this.InterfaceType = interfaceType;
                        isTypeChanged = true;
                    }
                }

                if (isAvailableChanged || isTypeChanged)
                    this.OnNetworkChanged(isAvailableChanged, isTypeChanged);
            }
            catch (Exception ex)
            {
                var handler = Error;

                if (handler != null)
                    Deployment.Current.Dispatcher.BeginInvoke(
                        () => handler(this, new NetworkErrorEventArgs(ex.Message, null, ex)));
            }
        }

        /// <summary>
        /// Called when [network changed].
        /// </summary>
        /// <param name="isAvailableChanged">if set to <c>true</c> [is available changed].</param>
        /// <param name="isTypeChanged">if set to <c>true</c> [is type changed].</param>
        private void OnNetworkChanged(bool isAvailableChanged, bool isTypeChanged)
        {
            EventHandler<NetworkChangedEventArgs> handler = this.NetworkChanged;

            if (handler != null)
                Deployment.Current.Dispatcher.BeginInvoke(
                    () => handler(this, new NetworkChangedEventArgs(isAvailableChanged, isTypeChanged)));
        }
    }

    public class NetworkChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is availability changed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is availability changed; otherwise, <c>false</c>.
        /// </value>
        public bool IsAvailabilityChanged { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is network type changed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is network type changed; otherwise, <c>false</c>.
        /// </value>
        public bool IsNetworkTypeChanged { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkChangedEventArgs"/> class.
        /// </summary>
        /// <param name="isAvailabilityChanged">if set to <c>true</c> [is availability changed].</param>
        /// <param name="isNetworkTypeChanged">if set to <c>true</c> [is network type changed].</param>
        public NetworkChangedEventArgs(bool isAvailabilityChanged, bool isNetworkTypeChanged)
        {
            this.IsAvailabilityChanged = isAvailabilityChanged;
            this.IsNetworkTypeChanged = isNetworkTypeChanged;
        }
    }
}
