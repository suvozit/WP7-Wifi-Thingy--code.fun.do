using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using PAARC.DataAcquisition;
using PAARC.Shared;
using PAARC.Shared.Data;
using PAARC.WP7.Commands;
using PAARC.WP7.Controls;
using PAARC.WP7.Services;
using Thickness = System.Windows.Thickness;

namespace PAARC.WP7.ViewModels
{
    public class InputPageViewModel : NavigationViewModelBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private TextInputHelper _textInputHelper;
        private string _lastInputText;

        private bool _isCustomDragInProgress;
        private bool _isTextInputInProgress;

        private Thickness _inputMarginThickness;
        private Visibility _textInputVisibility;
        private Visibility _customDragVisibility;

        private bool _backNavigationInProgress;
        private bool _waitForInitializedState;

        private DispatcherTimer _timer;
        private bool _isWaitingForConnection;
        private int _waitingSecondsRemaining;

        #region Properties for UI bindings

        public Thickness InputMarginThickness
        {
            get
            {
                return _inputMarginThickness;
            }
            set
            {
                if (_inputMarginThickness != value)
                {
                    _inputMarginThickness = value;
                    RaisePropertyChanged("InputMarginThickness");
                }
            }
        }

        public Visibility TextInputVisibility
        {
            get
            {
                return _textInputVisibility;
            }
            set
            {
                if (_textInputVisibility != value)
                {
                    _textInputVisibility = value;
                    RaisePropertyChanged("TextInputVisibility");
                }
            }
        }

        public Visibility CustomDragVisibility
        {
            get
            {
                return _customDragVisibility;
            }
            set
            {
                if (_customDragVisibility != value)
                {
                    _customDragVisibility = value;
                    RaisePropertyChanged("CustomDragVisibility");
                }
            }
        }

        public bool IsCustomDragInProgress
        {
            get
            {
                return _isCustomDragInProgress;
            }
            set
            {
                if (_isCustomDragInProgress != value)
                {
                    _isCustomDragInProgress = value;
                    RaisePropertyChanged("IsCustomDragInProgress");
                    RaisePropertyChanged("IsTextInputEnabled");
                }
            }
        }

        public bool IsCustomDragEnabled
        {
            get
            {
                return !IsTextInputInProgress;
            }
        }

        public bool IsTextInputInProgress
        {
            get
            {
                return _isTextInputInProgress;
            }
            set
            {
                if (_isTextInputInProgress != value)
                {
                    _isTextInputInProgress = value;
                    RaisePropertyChanged("IsTextInputInProgress");
                    RaisePropertyChanged("IsCustomDragEnabled");
                }
            }
        }

        public bool IsTextInputEnabled
        {
            get
            {
                return !IsCustomDragInProgress;
            }
        }

        public bool IsWaitingForConnection
        {
            get
            {
                return _isWaitingForConnection;
            }
            set
            {
                if (_isWaitingForConnection != value)
                {
                    _isWaitingForConnection = value;
                    RaisePropertyChanged("IsWaitingForConnection");
                    RaisePropertyChanged("WaitingForConnectionMessageVisibility");
                }
            }
        }

        public Visibility WaitingForConnectionMessageVisibility
        {
            get
            {
                return IsWaitingForConnection ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public string WaitingForConnectionMessage
        {
            get
            {
                return string.Format("Trying to reconnect...{0}{1}{2} seconds left{3}{4}Use the back button to prematurely abort and return to the main menu.",
                                     Environment.NewLine,
                                     Environment.NewLine,
                                     _waitingSecondsRemaining,
                                     Environment.NewLine,
                                     Environment.NewLine);
            }
        }

        public ICommand TextInputUserInteractionStartedCommand
        {
            get;
            private set;
        }

        public ICommand TextInputUserInteractionEndedCommand
        {
            get;
            private set;
        }

        public ICommand TextInputActivatedCommand
        {
            get;
            private set;
        }

        public ICommand CustomDragUserInteractionStartedCommand
        {
            get;
            private set;
        }

        public ICommand CustomDragUserInteractionEndedCommand
        {
            get;
            private set;
        }

        public ICommand CustomDragActivatedCommand
        {
            get;
            private set;
        }

        public ICommand CustomDragDeactivatedCommand
        {
            get;
            private set;
        }

        #endregion

        public InputPageViewModel()
        {
            _textInputHelper = new TextInputHelper();
            _textInputHelper.TextInputFinished += TextInputHelper_TextInputFinished;

            TextInputUserInteractionStartedCommand = new RelayCommand(TextInputUserInteractionStarted);
            TextInputUserInteractionEndedCommand = new RelayCommand(TextInputUserInteractionEnded);
            TextInputActivatedCommand = new RelayCommand(TextInputActivated);
            CustomDragUserInteractionStartedCommand = new RelayCommand(CustomDragUserInteractionStarted);
            CustomDragUserInteractionEndedCommand = new RelayCommand(CustomDragUserInteractionEnded);
            CustomDragActivatedCommand = new RelayCommand(CustomDragActivated);
            CustomDragDeactivatedCommand = new RelayCommand(CustomDragDeactivated);
        }

        #region Navigation Handling

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _logger.Trace("Setting up in OnNavigatedTo");

            base.OnNavigatedTo(e);

            // get controller and set up
            var controllerClient = ApplicationContext.Current.ControllerClient;
            controllerClient.StateChanged += ControllerClient_StateChanged;
            controllerClient.Error += ControllerClient_Error;
            controllerClient.ConfigurationChanged += ControllerClient_ConfigurationChanged;
            controllerClient.DataSource.DataTypesChanged += DataSource_DataTypesChanged;

            // let's see how we got here
            if (e.NavigationMode == NavigationMode.Back)
            {
                // => someone navigated back to us, which can only mean that we returned from deactivation
                // => check to see if we should try to reconnect
                var lastIpAddress = ApplicationContext.Current.LastServerAddress;
                if (controllerClient.Configuration.AutoReconnectOnActivation && lastIpAddress != null)
                {
                    InitiateReconnect();
                }
                else
                {
                    // no reconnect => navigate back to the main menu
                    _logger.Trace("Cannot or not supposed to reconnect => navigating back");

                    // we cannot or are not supposed to reconnect
                    NavigateBackToMainPage();
                }
            }
            else
            {
                // if we came here from the main page
                // we are connected
                Debug.Assert(controllerClient.State == PhoneControllerState.Ready);

                AdjustInputMarginThickness();

                // initial setup
                var activeDataTypes = controllerClient.DataSource.GetActiveDataTypes();
                AdjustDataTypesVisibility(activeDataTypes);
                InitializeDataSource(activeDataTypes);
            }
        }

        private void NavigateBackToMainPage()
        {
            // => navigate back to the main page
            var navigationService = ApplicationContext.Current.NavigationService;
            if (navigationService.CanGoBack && !_backNavigationInProgress)
            {
                _backNavigationInProgress = true;
                navigationService.GoBack();
            }
        }

        private void InitiateReconnect()
        {
            // this is a workaround for a nasty side effect on WP7 that is described here:
            // http://www.pitorque.de/MisterGoodcat/post/Windows-Phone-7-Pitfall-The-Dispatcher-and-Deactivation.aspx
            _waitForInitializedState = true;

            // start the timer to give some visual feedback
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(1.0);
                _timer.Tick += ConnectionWaitingTimer_Tick;
            }

            IsWaitingForConnection = true;
            _waitingSecondsRemaining = 20;
            RaisePropertyChanged("WaitingForConnectionMessage");
            _timer.Start();

            // see if we have a valid connection and can try to reconnect directly
            var watchdog = NetworkWatchdog.Current;
            var canConnect = watchdog != null && watchdog.IsNetworkAvailable && watchdog.InterfaceType == NetworkInterfaceType.Wireless80211;

            if (canConnect)
            {
                Reconnect();
            }
        }

        private void ConnectionWaitingTimer_Tick(object sender, EventArgs e)
        {
            // update UI
            _waitingSecondsRemaining--;
            RaisePropertyChanged("WaitingForConnectionMessage");

            // decide what to do
            if (_waitingSecondsRemaining == 0)
            {
                // we were not successful
                _timer.Stop();
                IsWaitingForConnection = false;

                MessageBox.Show("No suitable network connection available to automatically reconnect. Navigating back to the main menu...",
                                "Reconnect failed",
                                MessageBoxButton.OK);

                NavigateBackToMainPage();
            }
            else
            {
                // check whether we are already trying to reconnect or not
                if (ApplicationContext.Current.ControllerClient.State == PhoneControllerState.Closed)
                {
                    // we are not yet trying to reconnect => check whether we have a suitable network
                    var watchdog = NetworkWatchdog.Current;
                    var canConnect = watchdog != null && watchdog.IsNetworkAvailable && watchdog.InterfaceType == NetworkInterfaceType.Wireless80211;

                    if (canConnect)
                    {
                        Reconnect();
                    }
                }
            }
        }

        private void Reconnect()
        {
            var controllerClient = ApplicationContext.Current.ControllerClient;

            // get ip and try to reconnect
            var lastIpAddress = ApplicationContext.Current.LastServerAddress;

            try
            {
                _logger.Trace("Trying to reconnect automatically");
                
                // start connection
                controllerClient.ConnectAsync(lastIpAddress);
            }
            catch (Exception)
            {
                // shut down and out of here
                controllerClient.Shutdown();
                NavigateBackToMainPage();
            }
        }

        private void InitializeDataSource(IEnumerable<DataType> activeDataTypes)
        {
            if (activeDataTypes.Contains(DataType.CustomDrag))
            {
                // pause this, because we manually trigger it when the user wants to explicitly input that type of gesture
                ApplicationContext.Current.ControllerClient.DataSource.PauseAcquisition(DataType.CustomDrag);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _logger.Trace("Cleaning up in OnNavigatedFrom");

            base.OnNavigatedFrom(e);

            // remove event handlers
            var controllerClient = ApplicationContext.Current.ControllerClient;
            controllerClient.StateChanged -= ControllerClient_StateChanged;
            controllerClient.Error -= ControllerClient_Error;
            controllerClient.ConfigurationChanged -= ControllerClient_ConfigurationChanged;
            controllerClient.DataSource.DataTypesChanged -= DataSource_DataTypesChanged;

            // stop timer if applicable
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
            IsWaitingForConnection = false;

            // disconnect when we return to the main page
            if (e.NavigationMode == NavigationMode.Back)
            {
                try
                {
                    // disconnect the connection when we return to the main page
                    controllerClient.Shutdown();
                }
                catch (PhoneControllerException)
                {
                    // ignore that kind of errors
                }
            }
        }

        #endregion

        #region Processing of Controller events

        private void ControllerClient_StateChanged(object sender, PhoneControllerStateEventArgs e)
        {
            // this is a workaround for a nasty side effect on WP7, described here:
            // http://www.pitorque.de/MisterGoodcat/post/Windows-Phone-7-Pitfall-The-Dispatcher-and-Deactivation.aspx
            if (_waitForInitializedState && e.State != PhoneControllerState.Initialized)
            {
                return;
            }
            _waitForInitializedState = false;

            // let's see what we should do
            switch (e.State)
            {
                case PhoneControllerState.Closed:
                    NavigateBackToMainPage();
                    break;
                case PhoneControllerState.Ready:
                    // stop timer if applicable
                    if (_timer != null)
                    {
                        _timer.Stop();
                        _timer = null;
                    }
                    IsWaitingForConnection = false;

                    // initial setup
                    AdjustInputMarginThickness();
                    var activeDataTypes = ApplicationContext.Current.ControllerClient.DataSource.GetActiveDataTypes();
                    AdjustDataTypesVisibility(activeDataTypes);
                    InitializeDataSource(activeDataTypes);
                    break;
            }
        }

        private void ControllerClient_Error(object sender, ErrorEventArgs e)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
            IsWaitingForConnection = false;

            // show to the user
            MessageBox.Show("An unexpected error occurred: " + e.Error.Message);

            NavigateBackToMainPage();
        }

        private void ControllerClient_ConfigurationChanged(object sender, EventArgs e)
        {
            _logger.Trace("Received ConfigurationChanged event from controller client");

            AdjustInputMarginThickness();
        }

        private void DataSource_DataTypesChanged(object sender, DataTypesChangedEventArgs e)
        {
            _logger.Trace("Received DataTypesChanged event from controller client");

            AdjustDataTypesVisibility(e.NewDataTypes);
            InitializeDataSource(e.NewDataTypes);
        }

        private void AdjustDataTypesVisibility(IEnumerable<DataType> newDataTypes)
        {
            TextInputVisibility = newDataTypes.Contains(DataType.Text) ? Visibility.Visible : Visibility.Collapsed;
            CustomDragVisibility = newDataTypes.Contains(DataType.CustomDrag) ? Visibility.Visible : Visibility.Collapsed;
        }

        public void AdjustInputMarginThickness()
        {
            var inputMargin = ApplicationContext.Current.ControllerClient.Configuration.TouchInputMargin;

            // depending on the screen orientation, we switch the left and right margins
            // => landscape left is our "normal" mode, landscape right needs switched margins because
            //    the special input buttons also are not rotated but mirrored
            switch (DeviceInfo.Current.Orientation)
            {
                case PageOrientation.LandscapeLeft:
                    InputMarginThickness = new Thickness(inputMargin.Left, inputMargin.Top, inputMargin.Right, inputMargin.Bottom);
                    break;
                case PageOrientation.LandscapeRight:
                    InputMarginThickness = new Thickness(inputMargin.Right, inputMargin.Top, inputMargin.Left, inputMargin.Bottom);
                    break;
            }
        }

        #endregion

        #region Text Input

        private void TextInputUserInteractionStarted()
        {
            _logger.Trace("User interaction with text input grabber started");

            PauseAllDataAcquisition();
        }

        private void TextInputUserInteractionEnded()
        {
            _logger.Trace("User interaction with text input grabber ended");

            // if the user is inputting text, do nothing
            if (IsTextInputInProgress)
            {
                return;
            }

            // resume data acquisition after the interaction with the text input control
            // if the user did not manage or want to bring up the text input control
            ResumeDataAcquisition();
        }

        private void TextInputActivated()
        {
            _logger.Trace("Activating text input");

            // play sound
            SoundPlayer.Play(SoundPlayer.SelectSound);

            // simply show the text input prompt, it will do the rest
            _textInputHelper.ShowTextInputPrompt(_lastInputText);
        }

        private void TextInputHelper_TextInputFinished(object sender, TextInputEventArgs e)
        {
            // make sure the UI is notified about the fact
            // that text input has finished
            IsTextInputInProgress = false;

            // make sure we resume all paused data types (including text)
            ResumeDataAcquisition();

            // do we need to do anything?
            if (string.IsNullOrEmpty(e.Text))
            {
                _logger.Trace("No text entered, nothing to do");
                return;
            }

            _logger.Trace("Sending manually entered text");

            // store for next time
            _lastInputText = e.Text;

            try
            {
                // send!
                var textData = new TextData();
                textData.Text = e.Text;
                ApplicationContext.Current.ControllerClient.DataSource.AddData(textData);
            }
            catch (PhoneControllerException ex)
            {
                MessageBox.Show("An error occurred while sending the text: " + ex.Message);
            }
        }

        #endregion

        #region Drag Input

        private void CustomDragUserInteractionStarted()
        {
            Debug.WriteLine("Input Page: CustomDrag Interaction Started");

            _logger.Trace("User interaction with the drag grabber started");

            PauseAllDataAcquisition();
        }

        private void CustomDragUserInteractionEnded()
        {
            Debug.WriteLine("Input Page: CustomDrag Interaction Ended, in progress: " + IsCustomDragInProgress);

            _logger.Trace("User interaction with the drag grabber ended");

            // if the user has activated the custom drag, we do nothing
            if (IsCustomDragInProgress)
            {
                return;
            }

            // the user ended interacting with the custom drag grabber
            // without actually activating custom dragging => make sure to 
            // resume all paused data acquisitions
            ResumeDataAcquisition();
        }

        private void CustomDragActivated()
        {
            Debug.WriteLine("Input Page: CustomDrag Activated");

            _logger.Trace("Activating custom drag gestures");

            // play sound
            SoundPlayer.Play(SoundPlayer.SelectSound);

            // resume acquisistion of drag data
            var dataSource = ApplicationContext.Current.ControllerClient.DataSource;
            dataSource.ResumeAcquisition(DataType.CustomDrag);
            dataSource.ResumeAcquisition(DataType.CustomDragComplete);
        }

        private void CustomDragDeactivated()
        {
            Debug.WriteLine("Input Page: CustomDrag Deactivated");

            _logger.Trace("Deactivating custom drag gestures");

            // play sound
            SoundPlayer.Play(SoundPlayer.DeselectSound);

            // once the custom dragging is deactivated, we manually send 
            // a complete message, then pause data acquisition of the dragging,
            // and finally resume all previously paused data acquisition
            var dataSource = ApplicationContext.Current.ControllerClient.DataSource;

            try
            {
                // manually send a drag complete message
                var dragCompleteData = new CustomDragCompleteData();
                dataSource.AddData(dragCompleteData);
            }
            catch (PhoneControllerException ex)
            {
                MessageBox.Show("An error occurred while finishing sending drag data: " + ex);
            }

            // pause dragging
            dataSource.PauseAcquisition(DataType.CustomDrag);
            dataSource.PauseAcquisition(DataType.CustomDragComplete);

            // resume (does not resume dragging)
            ResumeDataAcquisition();
        }

        private void PauseAllDataAcquisition()
        {
            // get active data acquisitions and pause all
            var dataSource = ApplicationContext.Current.ControllerClient.DataSource;
            var activeDataTypes = dataSource.GetActiveDataTypes();
            foreach (var dataType in activeDataTypes)
            {
                // stop all others
                dataSource.PauseAcquisition(dataType);
            }
        }

        private void ResumeDataAcquisition()
        {
            var dataSource = ApplicationContext.Current.ControllerClient.DataSource;
            var pausedDataTypes = dataSource.GetPausedDataTypes();
            foreach (var dataType in pausedDataTypes)
            {
                // ignore the custom drag inputs, these are supposed to be paused until explicitly activated above
                if (dataType == DataType.CustomDrag || dataType == DataType.CustomDragComplete)
                {
                    continue;
                }

                dataSource.ResumeAcquisition(dataType);
            }
        }

        #endregion
    }
}
