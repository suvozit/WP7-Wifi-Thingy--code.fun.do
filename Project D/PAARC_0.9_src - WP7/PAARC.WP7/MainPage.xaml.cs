using System;
using System.Windows;
using PAARC.WP7.Services;
using PAARC.WP7.Views;
using YourLastOptionsDialog;

namespace PAARC.WP7
{
    public partial class MainPage : NavigationPhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void ApplicationBarHelpButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarSettingsButton_Click(object sender, EventArgs e)
        {
            OptionsService.Current.Show(ApplicationContext.Current.Settings);
        }

        private void AdvancedOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            AdvancedOptionsPanel.Visibility = AdvancedOptionsPanel.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            AdvancedOptionsButton.Content = AdvancedOptionsPanel.Visibility == Visibility.Collapsed ? "Show Advanced Options" : "Hide Advanced Options";
        }
    }
}