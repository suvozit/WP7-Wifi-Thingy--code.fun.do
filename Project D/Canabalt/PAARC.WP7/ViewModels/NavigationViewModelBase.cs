using System.Windows.Navigation;

namespace PAARC.WP7.ViewModels
{
    /// <summary>
    /// A view model base class that abstracts the navigation features of Silverlight's page model.
    /// </summary>
    public class NavigationViewModelBase : ViewModelBase
    {
        protected virtual void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
        }

        protected virtual void OnNavigatedFrom(NavigationEventArgs e)
        {
        }

        protected virtual void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        internal void InternalNavigatingFrom(NavigatingCancelEventArgs e)
        {
            OnNavigatingFrom(e);
        }

        internal void InternalNavigatedFrom(NavigationEventArgs e)
        {
            OnNavigatedFrom(e);
        }

        internal void InternalNavigatedTo(NavigationEventArgs e)
        {
            OnNavigatedTo(e);
        }
    }
}
