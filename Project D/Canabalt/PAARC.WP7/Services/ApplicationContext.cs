
using System;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using Microsoft.Phone.Shell;
using PAARC.Communication;
using PAARC.DataAcquisition;
using PAARC.Shared;
using PAARC.WP7.Navigation;
using YourLastOptionsDialog;

namespace PAARC.WP7.Services
{
    /// <summary>
    /// A service that provides central access points to key components of the application, for example
    /// the controller client, navigation service and application settings.
    /// </summary>
    public class ApplicationContext : IApplicationService
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private const string LastServerAddressKey = "LastServerAddress";
        private const string ControllerConfigurationKey = "ControllerConfiguration";
        private const string ApplicationSettingsKey = "ApplicationSettings";
        private const string InitialMessageShownKey = "InitialMessageShown";

        private readonly DataSource _dataAcquirer;

        /// <summary>
        /// The current (pseudo-singleton) instance of the context.
        /// </summary>
        public static ApplicationContext Current
        {
            get;
            private set;
        }

        /// <summary>
        /// A navigation service implementation that can be used for navigation from view models, for example.
        /// </summary>
        public INavigationService NavigationService
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the controller client.
        /// </summary>
        public PhoneControllerClient ControllerClient
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the last known good server address.
        /// </summary>
        public IPAddress LastServerAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the application and user settings.
        /// </summary>
        public Settings Settings
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the initial user message has already been shown or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the initial message has been shown; otherwise, <c>false</c>.
        /// </value>
        public bool InitialMessageShown
        {
            get
            {
                // determine whether we need to show the initial hint or not
                return (IsolatedStorageSettings.ApplicationSettings.Contains(InitialMessageShownKey)
                        && (bool)IsolatedStorageSettings.ApplicationSettings[InitialMessageShownKey]);
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[InitialMessageShownKey] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationContext"/> class.
        /// </summary>
        public ApplicationContext()
        {
            // is null in design time mode
            if (PhoneApplicationService.Current != null)
            {
                PhoneApplicationService.Current.Launching += PhoneApplicationService_Launching;
                PhoneApplicationService.Current.Activated += PhoneApplicationService_Activated;
                PhoneApplicationService.Current.Deactivated += PhoneApplicationService_Deactivated;
                PhoneApplicationService.Current.Closing += PhoneApplicationService_Closing;
            }

            // create required objects
            _dataAcquirer = new DataSource();
            ControllerClient = new PhoneControllerClient(_dataAcquirer);
            Settings = new Settings();
        }

        /// <summary>
        /// Called by an application in order to initialize the application extension service.
        /// </summary>
        /// <param name="context">Provides information about the application state.</param>
        public void StartService(ApplicationServiceContext context)
        {
            // set the "singleton"
            Current = this;

            // get the root frame and use it for the navigation
            var app = Application.Current as App;
            if (app != null)
            {
                var frame = app.RootFrame;
                NavigationService = new FrameNavigationService(frame);
            }
        }

        /// <summary>
        /// Called by an application in order to stop the application extension service.
        /// </summary>
        public void StopService()
        {
        }

        private void PhoneApplicationService_Launching(object sender, LaunchingEventArgs e)
        {
            // set the property value so the app can offer convenient features for the user
            if (IsolatedStorageSettings.ApplicationSettings.Contains(LastServerAddressKey))
            {
                LastServerAddress = IPAddress.Parse(IsolatedStorageSettings.ApplicationSettings[LastServerAddressKey].ToString());
            }

            // restore settings
            if (IsolatedStorageSettings.ApplicationSettings.Contains(ApplicationSettingsKey))
            {
                try
                {
                    Settings = (Settings)IsolatedStorageSettings.ApplicationSettings[ApplicationSettingsKey];
                }
                catch (Exception)
                {
                    _logger.Trace("Restore of application settings from isolated storage failed.");
                    Settings = new Settings();
                }
            }
        }

        private void PhoneApplicationService_Activated(object sender, ActivatedEventArgs e)
        {
            _logger.Trace("Application activating");

            try
            {
                // try to restore some values
                if (IsolatedStorageSettings.ApplicationSettings.Contains(LastServerAddressKey))
                {
                    LastServerAddress = IPAddress.Parse(IsolatedStorageSettings.ApplicationSettings[LastServerAddressKey].ToString());
                }

                if (IsolatedStorageSettings.ApplicationSettings.Contains(ControllerConfigurationKey))
                {
                    var configuration = (ControllerConfiguration)IsolatedStorageSettings.ApplicationSettings[ControllerConfigurationKey];
                    ControllerClient.Configure(configuration);
                }
            }
            catch (Exception ex)
            {
                _logger.Trace("Reading isolated storage communication settings failed: {0}", ex);
            }

            // only restore application settings if we were tombstoned
            if (!e.IsApplicationInstancePreserved)
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(ApplicationSettingsKey))
                {
                    try
                    {
                        Settings = (Settings)IsolatedStorageSettings.ApplicationSettings[ApplicationSettingsKey];

                        // restore options service
                        OptionsService.Current.RestoreAfterTombstoning(Settings);
                    }
                    catch (Exception)
                    {
                        _logger.Trace("Restore of application settings from isolated storage failed.");
                        Settings = new Settings();
                    }
                }
            }
        }

        private void PhoneApplicationService_Deactivated(object sender, DeactivatedEventArgs e)
        {
            _logger.Trace("Application deactivating; saving values and shutting down");

            // if we have a server address, store it
            if (ControllerClient.ServerAddress != null)
            {
                IsolatedStorageSettings.ApplicationSettings[LastServerAddressKey] = ControllerClient.ServerAddress.ToString();
            }

            // also store the configuration,
            if (ControllerClient.Configuration != null)
            {
                IsolatedStorageSettings.ApplicationSettings[ControllerConfigurationKey] = ControllerClient.Configuration;
            }

            // and the application settings
            IsolatedStorageSettings.ApplicationSettings[ApplicationSettingsKey] = Settings;

            // ... and save
            IsolatedStorageSettings.ApplicationSettings.Save();

            // shut down the controller when we're deactivated
            ControllerClient.Shutdown();
        }

        private void PhoneApplicationService_Closing(object sender, ClosingEventArgs e)
        {
            _logger.Trace("Application closing; saving values and shutting down");

            // if we have a server address, store it
            if (ControllerClient.ServerAddress != null)
            {
                IsolatedStorageSettings.ApplicationSettings[LastServerAddressKey] = ControllerClient.ServerAddress.ToString();
            }

            // and the application settings
            IsolatedStorageSettings.ApplicationSettings[ApplicationSettingsKey] = Settings;

            // save!
            IsolatedStorageSettings.ApplicationSettings.Save();

            // shut down the controller when we're closing
            ControllerClient.Shutdown();
        }
    }
}
