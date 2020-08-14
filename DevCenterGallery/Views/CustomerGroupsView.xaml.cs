using DevCenterGallary.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCenterGallary.Views
{
    public sealed partial class CustomerGroupsView : Page
    {
        private CustomerGroupsViewModel _vm = new CustomerGroupsViewModel();

        public CustomerGroupsView()
        {
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
        }
    }
}
