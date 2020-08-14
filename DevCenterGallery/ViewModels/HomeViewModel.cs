using DevCenterGallary.Common.Services;
using DevCenterGallary.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using Windows.UI.Popups;

namespace DevCenterGallary.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private StoreService _storeService = null;
        private ICookieService _cookieService = null;

        public static BusyViewModel BusyVM { get; private set; } = new BusyViewModel();

        public HomeViewModel()
        {
            _registerDepedencies();
            _storeService = SimpleIoc.Default.GetInstance<StoreService>();
            _cookieService = SimpleIoc.Default.GetInstance<ICookieService>();
        }

        private void _registerDepedencies()
        {
            SimpleIoc.Default.Register<ICookieService,PersonalCookieService>();
            SimpleIoc.Default.Register<StoreService>();
        }

        public List<PageModel> Pages { get; set; } = new List<PageModel>()
        {
            new PageModel { PageName= "Flights", PageType= typeof(Views.FlightsView) },
            new PageModel{  PageName= "Costmer Group",  PageType = typeof(Views.CustomerGroupsView) }
        };

        private PageModel _selectedPage;
        public PageModel SelectedPage
        {
            get => _selectedPage;
            set => Set(ref _selectedPage, value);
        }

        private bool _isSignedIn = false;
        public bool IsSignedIn
        {
            get => _isSignedIn;
            set => Set(ref _isSignedIn, value);
        }


        public async void Login()
        {
            try
            {
                BusyVM.IsProcessing = true;
                await _cookieService.GetDevCenterCookie();
                IsSignedIn = true;
            }
            catch(Exception e)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                new MessageDialog(e.Message).ShowAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            finally
            {
                BusyVM.IsProcessing = false;
            }
        }

        private bool _isPersonalCookie = false;
        public bool IsPersonalCookie
        {
            get => _isPersonalCookie;
            set => Set(ref _isPersonalCookie, value);
        }

        public void SwitchCookieService()
        {
            SimpleIoc.Default.Unregister<ICookieService>();
            SimpleIoc.Default.Register<ICookieService, PersonalCookieService>();

            _cookieService = SimpleIoc.Default.GetInstance<ICookieService>();

            IsPersonalCookie = true;
        }
    }
}
