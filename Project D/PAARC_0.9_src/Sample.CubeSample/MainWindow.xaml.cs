using System.ComponentModel;
using System.Windows;
using Cube.ViewModels;

namespace Cube
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var vm = DataContext as MainWindowViewModel;
                if (vm != null)
                {
                    vm.InitializeController();
                }
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            // on closing, shut down the controller in the view model
            var vm = DataContext as MainWindowViewModel;
            if (vm != null)
            {
                vm.Shutdown();
            }
        }
    }
}
