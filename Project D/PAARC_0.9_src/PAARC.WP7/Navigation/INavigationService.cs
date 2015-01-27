using System;
using System.Windows.Navigation;

namespace PAARC.WP7.Navigation
{
    /// <summary>
    /// A simple abstraction of the navigation features of Silverlight's navigation service.
    /// </summary>
    public interface INavigationService
    {
        event NavigatingCancelEventHandler Navigating;
        event NavigatedEventHandler Navigated;
        bool CanGoBack
        {
            get;
        }
        bool Navigate(Uri source);
        void GoBack();
    }
}
