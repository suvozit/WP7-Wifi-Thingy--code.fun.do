
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Windows;
using PAARC.Communication;
using PAARC.Shared;
using PAARC.Shared.ControlCommands;
using PAARC.Shared.Data;
using PCController.Input;
using PCController.Update;
using PCController.Views;

namespace PCController.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Configuration _configuration;
        private RemoteUpdate _remoteUpdate;
        private PhoneControllerServer _phoneController;
        private InputEmulator _inputEmulator;
        private bool _isPhoneDeviceReady;
        private string _status;
        private string _ipAddress;
        private readonly ObservableCollection<string> _errors = new ObservableCollection<string>();

        #region Properties for UI binding

        /// <summary>
        /// Gets the application version.
        /// </summary>
        public string ApplicationVersion
        {
            get
            {
                var versionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return string.Format("version {0}", versionNumber);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a client (phone device) has connected successfully 
        /// and is ready to receive control commands and send data.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is phone device ready; otherwise, <c>false</c>.
        /// </value>
        public bool IsPhoneDeviceReady
        {
            get
            {
                return _isPhoneDeviceReady;
            }
            set
            {
                if (_isPhoneDeviceReady != value)
                {
                    _isPhoneDeviceReady = value;
                    RaisePropertyChanged("IsPhoneDeviceReady");
                }
            }
        }

        /// <summary>
        /// Gets or sets a human-readable status of the controller.
        /// </summary>
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    RaisePropertyChanged("Status");
                }
            }
        }

        /// <summary>
        /// Gets or sets the IP address of the current computer that is used to receive client connections.
        /// Displayed in the UI as a convenience to enable the user to easily connect to the address using
        /// the phone if they don't use the discovery mode.
        /// </summary>
        public string IPAddress
        {
            get
            {
                return _ipAddress;
            }
            set
            {
                if (_ipAddress != value)
                {
                    _ipAddress = value;
                    RaisePropertyChanged("IPAddress");
                }
            }
        }

        /// <summary>
        /// Gets or sets the configuration that is used to configure connected phone clients.
        /// The individual properties control the detail behavior of the client.
        /// </summary>
        public Configuration Configuration
        {
            get
            {
                return _configuration;
            }
            set
            {
                if (_configuration != value)
                {
                    if (_configuration != null)
                    {
                        _configuration.PropertyChanged -= Configuration_PropertyChanged;
                    }

                    _configuration = value;

                    if (_configuration != null)
                    {
                        _configuration.PropertyChanged += Configuration_PropertyChanged;
                    }

                    RaisePropertyChanged("Configuration");
                }
            }
        }

        /// <summary>
        /// Gets the collection of errors that occurred during the runtime of the application.
        /// </summary>
        public ObservableCollection<string> Errors
        {
            get
            {
                return _errors;
            }
        }

        #endregion

        public MainWindowViewModel()
        {
            // create all required objects
            _phoneController = new PhoneControllerServer();
            _phoneController.Error += PhoneController_Error;
            _phoneController.StateChanged += PhoneController_StateChanged;
            _phoneController.DataMessageReceived += PhoneController_DataMessageReceived;

            _inputEmulator = new InputEmulator();

            // make sure to load the previous configuration
            // so the user does not need to set all desired options every time
            // the application starts
            Configuration = new Configuration();
            Configuration.Load();

            // this will check for new versions on startup
            _remoteUpdate = new RemoteUpdate();
            _remoteUpdate.VersionDataDownloaded += RemoteUpdate_VersionDataDownloaded;
            _remoteUpdate.Error += RemoteUpdate_Error;

            Status = "Started";
        }

        #region Remote Update Check

        private void RemoteUpdate_VersionDataDownloaded(object sender, EventArgs e)
        {
            try
            {
                var newestAvailableVersion = _remoteUpdate.AvailableVersions.FirstOrDefault();
                if (newestAvailableVersion == null)
                {
                    return;
                }

                if (VersionHelper.IsNewerThanCurrentVersion(newestAvailableVersion.VersionNumber))
                {
                    // a newer version is available
                    // => show info (a better way would be to use a factory or similar to get the view)
                    var vm = new VersionInfoWindowViewModel(_remoteUpdate.AvailableVersions);
                    var view = new VersionInfoWindow();
                    view.DataContext = vm;
                    view.Owner = Application.Current.MainWindow;
                    view.ShowDialog();
                }
            }
            finally
            {
                // disable remote update until next app start
                _remoteUpdate = null;
            }
        }

        private void RemoteUpdate_Error(object sender, EventArgs e)
        {
            try
            {
                AddError(_remoteUpdate.ErrorMessage);
            }
            finally
            {
                // disable remote update until next app start
                _remoteUpdate = null;
            }
        }

        #endregion

        #region Configuration and Controlling the Client

        private void Configuration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // depending on what configuration setting changes
            // we invoke the corresponding dedicated method
            switch (e.PropertyName)
            {
                case "UseAccelerometer":
                    InitializeAccelerometer();
                    break;
                case "UseTouchInput":
                    InitializeTouch();
                    break;
                case "TouchInputIsRelative":
                    _inputEmulator.UseRelativeTouchInput = Configuration.TouchInputIsRelative;
                    break;
                case "UseTapGestures":
                    InitializeTapGestures();
                    break;
                case "UseDoubleTapGestures":
                    InitializeDoubleTapGestures();
                    break;
                case "UseHoldGestures":
                    InitializeHoldGestures();
                    break;
                case "UseFlickGestures":
                    InitializeFlickGestures();
                    break;
                case "UseTextInput":
                    InitializeTextInput();
                    break;
                case "UseDragGestures":
                    InitializeDragGestures();
                    break;
                case "EnableTracing":
                    LoggingConfigurator.Configure(Configuration.EnableTracing, Configuration.TracingEndpointAddress);
                    InitializeConfiguration();
                    break;
                case "TracingEndpointAddress":
                    if (Configuration.EnableTracing)
                    {
                        LoggingConfigurator.Configure(Configuration.EnableTracing, Configuration.TracingEndpointAddress);
                    }
                    InitializeConfiguration();
                    break;
            }
        }

        private void PhoneController_StateChanged(object sender, PhoneControllerStateEventArgs e)
        {
            IsPhoneDeviceReady = e.State == PhoneControllerState.Ready;

            // depending on the current phone controller state, we perform different actions:
            // * try to re-initialize if the controller has closed
            // * send the initial configuration and activate/deactivate data acquisition when the controller is ready
            // * update our status display in every case
            switch (e.State)
            {
                case PhoneControllerState.Initialized:
                    Status = "Waiting for a client connection...";
                    break;
                case PhoneControllerState.Ready:
                    Status = "Phone is connected!";

                    // send config
                    InitializeConfiguration();

                    // send all required commands
                    InitializeAccelerometer();                   
                    InitializeTouch();
                    InitializeTapGestures();
                    InitializeDoubleTapGestures();
                    InitializeHoldGestures();
                    InitializeFlickGestures();
                    InitializeTextInput();
                    InitializeDragGestures();
                    break;
                case PhoneControllerState.Closing:
                    Status = "Connection to phone is closing.";
                    break;
                case PhoneControllerState.Closed:
                    Status = "Connection to phone closed.";

                    // try to re-initialize
                    Initialize();
                    break;
                case PhoneControllerState.Error:
                    Status = "Error";
                    break;
            }
        }

        private void InitializeConfiguration()
        {
            if (_phoneController != null && _phoneController.State == PhoneControllerState.Ready)
            {
                // build and send the configuration, depending on what the user has chosen
                var command = ControlCommandFactory.CreateConfigurationCommand();
                command.Configuration.AutoReconnectOnActivation = true;
                command.Configuration.EnableTracing = Configuration.EnableTracing;
                command.Configuration.TracingEndpointAddress = Configuration.TracingEndpointAddress;

                try
                {
                    _phoneController.SendCommandAsync(command);
                }
                catch (Exception ex)
                {
                    AddError("Error while sending configuration command: " + ex.Message);
                }
            }
        }

        private void InitializeAccelerometer()
        {
            SendControlCommand(DataType.Accelerometer, Configuration.UseAccelerometer ? ControlCommandAction.Start : ControlCommandAction.Stop);
        }

        private void InitializeMotion()
        {
            SendControlCommand(DataType.Motion, Configuration.UseAccelerometer ? ControlCommandAction.Start : ControlCommandAction.Stop);
        }

        private void InitializeTouch()
        {
            SendControlCommand(DataType.Touch, Configuration.UseTouchInput ? ControlCommandAction.Start : ControlCommandAction.Stop);
        }

        private void InitializeTapGestures()
        {
            SendControlCommand(DataType.Tap, Configuration.UseTapGestures ? ControlCommandAction.Start : ControlCommandAction.Stop);
        }

        private void InitializeDoubleTapGestures()
        {
            SendControlCommand(DataType.DoubleTap, Configuration.UseDoubleTapGestures ? ControlCommandAction.Start : ControlCommandAction.Stop);
        }

        private void InitializeHoldGestures()
        {
            SendControlCommand(DataType.Hold, Configuration.UseHoldGestures ? ControlCommandAction.Start : ControlCommandAction.Stop);
        }

        private void InitializeFlickGestures()
        {
            SendControlCommand(DataType.Flick, Configuration.UseFlickGestures ? ControlCommandAction.Start : ControlCommandAction.Stop);
        }

        private void InitializeDragGestures()
        {
            SendControlCommand(DataType.CustomDrag, Configuration.UseDragGestures ? ControlCommandAction.Start : ControlCommandAction.Stop);
            SendControlCommand(DataType.CustomDragComplete, Configuration.UseDragGestures ? ControlCommandAction.Start : ControlCommandAction.Stop);
        }

        private void InitializeTextInput()
        {
            SendControlCommand(DataType.Text, Configuration.UseTextInput ? ControlCommandAction.Start : ControlCommandAction.Stop);
        }

        private void SendControlCommand(DataType dataType, ControlCommandAction action)
        {
            if (_phoneController != null && _phoneController.State == PhoneControllerState.Ready)
            {
                var command = ControlCommandFactory.CreateCommand(dataType, action);

                try
                {
                    _phoneController.SendCommandAsync(command);
                }
                catch (Exception ex)
                {
                    AddError(string.Format("Error while sending command '{0}, {1}': {2}", dataType, action, ex.Message));
                }
            }
        }

        #endregion

        private void PhoneController_Error(object sender, ErrorEventArgs e)
        {
            IsPhoneDeviceReady = _phoneController.State == PhoneControllerState.Ready;

            // add a message for the user...
            AddError(e.Error.Message);

            try
            {
                // ...then shut down. this eventually leads to a transition to the "Closed"
                // state, which in turn triggers a re-initialization of the controller
                // (in the PhoneController_StateChanged method).
                _phoneController.Shutdown();
            }
            catch (Exception ex)
            {
                AddError("Error while shutting down controller: " + ex.Message);
            }
        }

        private void PhoneController_DataMessageReceived(object sender, DataMessageEventArgs e)
        {
            // let all input messages be processed by the input emulator
            try
            {
                _inputEmulator.Process(e.DataMessage);
            }
            catch (Exception ex)
            {
                // that kind of input injection has some potential for failures, 
                // for example when the secure desktop (UAC) is active we cannot 
                // inject input that way and will receive an error.
                AddError(string.Format("Error while processing input data of type {0}: {1}", e.DataMessage.DataType, ex.Message));
            }
        }

        private void AddError(string error)
        {
            // make sure this doesn't get out of hands
            while (Errors.Count > 100)
            {
                Errors.RemoveAt(Errors.Count - 1);
            }

            var item = string.Format("{0} - {1}", DateTime.Now.ToString("u"), error);
            Errors.Insert(0, item);
        }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        public void Initialize()
        {
            try
            {
                if (_remoteUpdate != null)
                {
                    _remoteUpdate.DownloadVersionData(Configuration.RemoteUpdateSource);
                }
            }
            catch (Exception ex)
            {
                AddError("Error while trying to start downloading new version information: " + ex.Message);
            }

            try
            {
                var addresses = Dns.GetHostAddresses(string.Empty);
                var address = addresses.First(o => o.AddressFamily == AddressFamily.InterNetwork);
                IPAddress = address.ToString();
                _phoneController.Initialize(address);

                // make sure to pick up the initial configuration state
                // (at the moment there is only one setting to take care of)
                _inputEmulator.UseRelativeTouchInput = Configuration.TouchInputIsRelative;
            }
            catch (Exception ex)
            {
                // simply add a message; we do not shut down the controller from it's error
                // state, as this would trigger another re-initialization (in the PhoneController_StateChanged method)
                // and potentially result in an error loop.
                AddError("Initialization failed with the following error: " + ex.Message);
            }
        }

        /// <summary>
        /// Shuts down the controller and saves the configuration for next time.
        /// </summary>
        public void Shutdown()
        {
            try
            {
                if (_phoneController != null)
                {
                    _phoneController.Shutdown();
                }

                Configuration.Save();
            }
            catch (Exception ex)
            {
                AddError("Shutdown failed with the following error: " + ex.Message);
            }
        }
    }
}
