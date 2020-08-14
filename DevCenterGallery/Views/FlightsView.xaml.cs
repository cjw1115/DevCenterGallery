using DevCenterGallary.Common.Models;
using Windows.UI.Xaml.Controls;
using DevCenterGallary.ViewModels;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DevCenterGallary.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FlightsView : Page
    {
        private FlightsViewModel _vm = new FlightsViewModel();

        private bool _isLoaded = false;

        public FlightsView()
        {
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
            this.Loaded += FlightsView_Loaded;
        }

        private async void FlightsView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if(!_isLoaded)
            {
                await _vm.Init();
                _isLoaded = true;
            }
        }

        private void _generatePreinstallKitClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            
            var packageInfo = (sender as HyperlinkButton).DataContext as Package;
            _vm.RequestPreinstallKit(packageInfo);
        }
    }
}
