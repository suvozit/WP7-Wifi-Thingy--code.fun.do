using System.Windows;
using Coding4Fun.Phone.Controls;

namespace PAARC.WP7.Controls
{
    public class CancelOnlyMessagePrompt : MessagePrompt
    {
        public CancelOnlyMessagePrompt()
            : base()
        {
            // a bit hacky, but the base implementation 
            // does not allow a different way of accessing this button
            var okButton = ActionPopUpButtons[0];
            okButton.Visibility = Visibility.Collapsed;

            IsCancelVisible = true;
        }
    }
}
