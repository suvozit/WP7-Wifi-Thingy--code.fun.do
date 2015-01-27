
using Microsoft.Phone.Controls;
using PAARC.WP7.ViewModels;
using PAARC.WP7.Views;

namespace PAARC.WP7
{
    public partial class InputPage : NavigationPhoneApplicationPage
    {
        public InputPage()
        {
            InitializeComponent();
        }

        private void NavigationPhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            CustomDragGrabber.Orientation = e.Orientation;
            TextInputGrabber.Orientation = e.Orientation;

            var vm = DataContext as InputPageViewModel;
            if (vm != null)
            {
                vm.AdjustInputMarginThickness();
            }
        }
    }
}