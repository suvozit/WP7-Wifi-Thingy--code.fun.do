using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace PCController.Views
{
    /// <summary>
    /// Interaction logic for VersionInfoWindow.xaml
    /// </summary>
    public partial class VersionInfoWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionInfoWindow"/> class.
        /// </summary>
        public VersionInfoWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                var url = e.Uri.ToString();
                var psi = new ProcessStartInfo(url);
                Process.Start(psi);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while launching browser: " + ex.Message);
            }
        }
    }
}
