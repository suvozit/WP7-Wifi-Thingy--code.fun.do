
using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using PAARC.Shared;
using PAARC.WP7.Commands;
using PAARC.WP7.Controls;
using PAARC.WP7.Services;

namespace PAARC.WP7.ViewModels
{
    public class MainPageViewModel : NavigationViewModelBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private string _serverIPAddress;
        private bool _canConnect;
        private MessagePromptHelper _prompt;

        public string ServerIPAddress
        {
            get
            {
                return _serverIPAddress;
            }
            set
            {
                if (_serverIPAddress != value)
                {
                    _serverIPAddress = value;
                    RaisePropertyChanged("ServerIPAddress");
                }
            }
        }

        public bool CanConnect
        {
            get
            {
                return _canConnect;
            }
            set
            {
                if (_canConnect != value)
                {
                    _canConnect = value;
                    RaisePropertyChanged("CanConnect");
                }
            }
        }

        public ICommand ConnectCommand
        {
            get;
            private set;
        }

        public ICommand ConnectToAddressCommand
        {
            get;
            private set;
        }

        public MainPageViewModel()
        {
            _prompt = new MessagePromptHelper();
            _prompt.PromptClosed += MessagePrompt_Closed;

            ConnectCommand = new RelayCommand(Connect);
            ConnectToAddressCommand = new RelayCommand(ConnectToAddress);
        }

        private void Connect()
        {
            if (!CheckNetwork())
            {
                return;
            }

            _logger.Trace("Auto-connecting");

            try
            {
                ApplicationContext.Current.ControllerClient.ConnectAsync();

                ShowPrompt();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ConnectToAddress()
        {
            if (!CheckNetwork())
            {
                return;
            }

            _logger.Trace("Connecting to specific IP address {0}", ServerIPAddress);

            IPAddress ipAddress;
            if (string.IsNullOrEmpty(ServerIPAddress) || !IPAddress.TryParse(ServerIPAddress, out ipAddress))
            {
                MessageBox.Show("Please enter a valid IP address in the format 123.123.123.123.");
                return;
            }

            try
            {
                // go
                ApplicationContext.Current.ControllerClient.ConnectAsync(ipAddress);

                ShowPrompt();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CheckNetwork()
        {
            if (NetworkWatchdog.Current.InterfaceType == NetworkInterfaceType.Ethernet)
            {
                MessageBox.Show("Please disconnect your phone from your PC before connecting to a remote server.");
                return false;
            }

            return true;
        }

        private void ControllerClient_Error(object sender, ErrorEventArgs e)
        {
            HidePrompt();

            try
            {
                // shut down the controller
                ApplicationContext.Current.ControllerClient.Shutdown();
            }
            catch (PhoneControllerException)
            {
                // ignore if this error happens    
            }

            // give the message prompt (if applicable)
            // a chance to hide itself before we show a message box
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                var technicalDetails = e.Error.ErrorCode ?? e.Error.Message;
                var message = string.Format("An error occurred while communicating to the server. Please try again later.{0}Technical details: \"{1}\"",
                                            Environment.NewLine,
                                            technicalDetails);
                MessageBox.Show(message);
            });
        }

        private void ControllerClient_StateChanged(object sender, PhoneControllerStateEventArgs e)
        {
            _logger.Trace("Received StateChanged event of controller client");

            if (e.State == PhoneControllerState.Ready)
            {
                _logger.Trace("Navigating to page InputPage");

                HidePrompt();

                ApplicationContext.Current.NavigationService.Navigate(new Uri("/InputPage.xaml", UriKind.Relative));
            }
        }

        private void NetworkWatchdog_NetworkChanged(object sender, NetworkChangedEventArgs e)
        {
            UpdateNetworkStatus();
        }

        private void UpdateNetworkStatus()
        {
            // determine if we can connect
            var watchdog = NetworkWatchdog.Current;

            // explicitly include the network type "Ethernet" although it does _not_ allow connection if Zune is launched.
            // this allows us to check for the network type in the connect methods and show a meaningful message to the user
            // when they try to connect when they're connected to the PC (if we excluded the type here they would be stuck
            // without a clue).
            CanConnect = watchdog != null
                         && watchdog.IsNetworkAvailable
                         && (watchdog.InterfaceType == NetworkInterfaceType.Wireless80211 || watchdog.InterfaceType == NetworkInterfaceType.Ethernet);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _logger.Trace("Setting up in OnNavigatedTo");

            base.OnNavigatedTo(e);

            // get controller
            var controller = ApplicationContext.Current.ControllerClient;

            // hook events
            controller.Error += ControllerClient_Error;
            controller.StateChanged += ControllerClient_StateChanged;

            // set IP address to "last known good"
            if (controller.ServerAddress != null)
            {
                ServerIPAddress = controller.ServerAddress.ToString();
            }
            else if (ApplicationContext.Current.LastServerAddress != null)
            {
                ServerIPAddress = ApplicationContext.Current.LastServerAddress.ToString();
            }

            // hook event of network watchdog
            NetworkWatchdog.Current.NetworkChanged += NetworkWatchdog_NetworkChanged;

            // initialize 
            UpdateNetworkStatus();

            // message?
            if (!ApplicationContext.Current.InitialMessageShown)
            {
                MessageBox.Show("To use this application, you need to connect it to a desktop application that you can interact with, for example the \"Windows Phone PC Controller\"." + Environment.NewLine + "Download it for free from" + Environment.NewLine + Environment.NewLine + "http://www.pitorque.de/paarc" + Environment.NewLine + Environment.NewLine + "(You can always find this information and more on the help page of the app later.)",
                                "PAARC - First startup",
                                MessageBoxButton.OK);

                // not next time
                ApplicationContext.Current.InitialMessageShown = true;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _logger.Trace("Cleaning up in OnNavigatedFrom");

            base.OnNavigatedFrom(e);

            // get controller
            var controller = ApplicationContext.Current.ControllerClient;

            // remove event handler
            controller.Error -= ControllerClient_Error;
            controller.StateChanged -= ControllerClient_StateChanged;

            // remove event handler of network watchdog
            NetworkWatchdog.Current.NetworkChanged -= NetworkWatchdog_NetworkChanged;
        }

        #region Prompt

        private void ShowPrompt()
        {
            _prompt.Show("Connecting to the server. This may take a few seconds...");
        }

        private void HidePrompt()
        {
            _prompt.Hide();
        }

        private void MessagePrompt_Closed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            // when the prompt has been dismissed programmatically the client is shut down already
            // => in any other case (user hit back button or confirmed the dialog) shut down
            if (e.Result != MessagePromptHelper.ProgrammaticallyDismissedToken)
            {
                try
                {
                    // shut down 
                    ApplicationContext.Current.ControllerClient.Shutdown();
                }
                catch (PhoneControllerException)
                {
                    // ignore these kinds of errors
                }
            }
        }

        #endregion
    }
}
