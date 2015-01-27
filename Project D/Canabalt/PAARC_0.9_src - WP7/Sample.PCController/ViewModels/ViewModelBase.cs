
using System.ComponentModel;

namespace PCController.ViewModels
{
    /// <summary>
    /// A simple base class for view models that implements the <c>INotifyPropertyChanged</c> interface.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
