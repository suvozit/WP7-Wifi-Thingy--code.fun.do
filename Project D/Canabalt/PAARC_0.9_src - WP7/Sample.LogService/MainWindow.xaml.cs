using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.ServiceModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using NLog.LogReceiverService;

namespace Sample.LogService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static SolidColorBrush GreenBrush = new SolidColorBrush(Color.FromArgb(255, 192, 224, 164));
        private static SolidColorBrush RedBrush = new SolidColorBrush(Color.FromArgb(255, 224, 192, 192));
        private static SolidColorBrush BlackBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        private CustomLogReceiverForwardingService _logService;
        private ServiceHost _serviceHost;
        private readonly ObservableCollection<LogEntry> _logEvents = new ObservableCollection<LogEntry>();
        private TimeSpan? _wp7Shift;
        private string _serviceAddress;

        private TimeSpan WP7Shift
        {
            get
            {
                if (_wp7Shift == null)
                {
                    var value = SimpleApplicationSettings.GetValue("WP7Shift");
                    if (value == null)
                    {
                        _wp7Shift = TimeSpan.Zero;
                    }
                    else
                    {
                        long ticks;
                        if (long.TryParse(value, out ticks))
                        {
                            _wp7Shift = TimeSpan.FromTicks(ticks);
                        }
                        else
                        {
                            _wp7Shift = TimeSpan.Zero;
                        }
                    }
                }

                return _wp7Shift.Value;
            }
            set
            {
                if (_wp7Shift != value)
                {
                    _wp7Shift = value;
                    SimpleApplicationSettings.Save("WP7Shift", value.Ticks.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        private string ServiceAddress
        {
            get
            {
                if (_serviceAddress == null)
                {
                    var value = SimpleApplicationSettings.GetValue("ServiceAddress");
                    if (value == null)
                    {
                        value = "http://[IP address]:[Port]/LogReceiver.svc";
                    }

                    _serviceAddress = value;
                }

                return _serviceAddress;
            }
            set
            {
                if (_serviceAddress != value)
                {
                    _serviceAddress = value;
                    SimpleApplicationSettings.Save("ServiceAddress", value);
                }
            }
        }

        public CollectionViewSource LogEntries
        {
            get;
            private set;
        }

        public MainWindow()
        {
            InitializeComponent();

            Closed += MainWindow_Closed;

            LogEntries = new CollectionViewSource();
            var timestampSort = new SortDescription("CorrectedTimestamp", ListSortDirection.Ascending);
            LogEntries.SortDescriptions.Add(timestampSort);
            var indexSort = new SortDescription("Index", ListSortDirection.Ascending);
            LogEntries.SortDescriptions.Add(indexSort);
            LogEntries.Source = _logEvents;

            ShiftMillisecondsUpDown.Value = WP7Shift.TotalMilliseconds;
            ServiceAddressTextBox.Text = ServiceAddress;

            DataContext = this;
        }

        private void StartHostingService()
        {
            // clean up first
            if (_serviceHost != null)
            {
                _serviceHost.Close();
            }

            try
            {
                // host a custom service
                var uri = new Uri(ServiceAddress);

                // create a service, a service host and binding
                _logService = new CustomLogReceiverForwardingService();
                _logService.LogEventReceived += LogService_LogEventReceived;
                _serviceHost = new ServiceHost(_logService, uri);

                var behavior = _serviceHost.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                behavior.InstanceContextMode = InstanceContextMode.Single;

                var binding = new BasicHttpBinding();
                _serviceHost.AddServiceEndpoint(typeof(ILogReceiverServer), binding, uri);
                _serviceHost.Open();
            }
            catch (Exception ex)
            {
                if (_serviceHost != null)
                {
                    _serviceHost.Close();
                    _serviceHost = null;
                }

                MessageBox.Show("Error while starting up service: " + ex);
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (_serviceHost != null)
            {
                _serviceHost.Close();
            }
        }

        private void LogService_LogEventReceived(object sender, LogEventArgs e)
        {
            var entry = new LogEntry
                            {
                                Index = _logEvents.Count,
                                OriginalTimestamp = ConvertToDateTime((string)e.LogEvent.Properties["time"]),
                                ClientType = (string)e.LogEvent.Properties["ClientName"] == "WP7" ? ClientType.WP7 : ClientType.PC,
                                Logger = e.LogEvent.LoggerName,
                                Message = e.LogEvent.Message,
                                ThreadId = (string)e.LogEvent.Properties["threadid"]
                            };

            if (entry.ClientType == ClientType.WP7)
            {
                entry.CorrectedTimestamp = entry.OriginalTimestamp + WP7Shift;
            }
            else
            {
                entry.CorrectedTimestamp = entry.OriginalTimestamp;
            }

            _logEvents.Add(entry);
            LogGrid.ScrollIntoView(entry);
        }

        private DateTime ConvertToDateTime(string timeString)
        {
            var result = DateTime.ParseExact(timeString, "HH:mm:ss.ffff", CultureInfo.InvariantCulture);
            return result;
        }

        private void DataGrid_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            var logEntry = e.Row.DataContext as LogEntry;
            if (logEntry != null)
            {
                switch (logEntry.ClientType)
                {
                    case ClientType.PC:
                        e.Row.Background = RedBrush;
                        break;
                    case ClientType.WP7:
                        e.Row.Background = GreenBrush;
                        break;
                    default:
                        e.Row.Background = BlackBrush;
                        break;
                }
            }
        }

        private void StartHostingButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceAddress = ServiceAddressTextBox.Text;
            StartHostingService();
        }

        private void ShiftButton_Click(object sender, RoutedEventArgs e)
        {
            var shift = ShiftMillisecondsUpDown.Value;
            if (!shift.HasValue)
            {
                return;
            }

            WP7Shift = TimeSpan.FromMilliseconds(shift.Value);

            CorrectExistingWP7Timestamps();
        }

        private void CalculateShiftButton_Click(object sender, RoutedEventArgs e)
        {
            var twoSelected = false;
            if (LogGrid.SelectedItems.Count == 2)
            {
                twoSelected = true;
            }

            LogEntry firstSelected = null;
            LogEntry secondSelected = null;
            var twoDifferentTypesSelected = false;
            if (twoSelected)
            {
                firstSelected = LogGrid.SelectedItems[0] as LogEntry;
                secondSelected = LogGrid.SelectedItems[1] as LogEntry;

                if (firstSelected != null && secondSelected != null && firstSelected.ClientType != secondSelected.ClientType)
                {
                    twoDifferentTypesSelected = true;
                }
            }

            if (!twoSelected || !twoDifferentTypesSelected)
            {
                string message = "Please select one PC and one WP7 entries that are directly related and close to each other,{0}for example the send and receive entries of a control command. The time difference is then calculated automatically.{1}Please note that the software assumes the PC message is the master and adds 1 millisecond offset to the WP7 message.";
                message = string.Format(message, Environment.NewLine, Environment.NewLine);
                MessageBox.Show(message);
                return;
            }

            var pcEntry = firstSelected.ClientType == ClientType.PC ? firstSelected : secondSelected;
            var wp7Entry = firstSelected.ClientType == ClientType.WP7 ? firstSelected : secondSelected;
            var diff = (pcEntry.OriginalTimestamp - wp7Entry.OriginalTimestamp).TotalMilliseconds;
            diff += diff > 0 ? -1.0 : 1.0;

            ShiftMillisecondsUpDown.Value = diff;
            WP7Shift = TimeSpan.FromMilliseconds(diff);
            CorrectExistingWP7Timestamps();
        }

        private void CorrectExistingWP7Timestamps()
        {
            // now the time-consuming part: correct all existing
            using (LogEntries.DeferRefresh())
            {
                foreach (var entry in _logEvents)
                {
                    if (entry.ClientType == ClientType.WP7)
                    {
                        entry.CorrectedTimestamp = entry.OriginalTimestamp + WP7Shift;
                    }
                }
            }
        }

        private void CopyToClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: implement a CSV export or similar
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            using (LogEntries.DeferRefresh())
            {
                _logEvents.Clear();
            }
        }
    }
}
