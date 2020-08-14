using DevCenterGallary.ViewModels;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DevCenterGallary
{
    public sealed partial class HomePage : Page
    {
        private HomeViewModel _vm = new HomeViewModel();

        public HomePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown; ;
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.S)
            {
                var ctrlState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Control);
                var shiftState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift);
                if (ctrlState == CoreVirtualKeyStates.Down && shiftState == CoreVirtualKeyStates.Down)
                {
                    args.Handled = true;
                    _vm.SwitchCookieService();
                }
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems!=null && e.AddedItems.Count>0)
            {
                var newPage = (Models.PageModel)e.AddedItems[0];
                frameNavigation.Navigate(newPage.PageType);
            }
        }
    }
}
