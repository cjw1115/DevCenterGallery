using DevCenterGallary.Common.Models;
using DevCenterGallary.Common.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace DevCenterGallary.ViewModels
{
    public class FlightsViewModel: ViewModelBase
    {
        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get => _products;
            set => Set(ref _products, value);
        }

        private ObservableCollection<Submission> _flights;
        public ObservableCollection<Submission> Flights
        {
            get => _flights;
            set => Set(ref _flights, value);
        }

        private ObservableCollection<Package> _flightPackages;
        public ObservableCollection<Package> FlightPackages
        {
            get => _flightPackages;
            set => Set(ref _flightPackages, value);
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get { return _selectedProduct; }
            set { Set(ref _selectedProduct, value); }
        }

        private Submission _selectedFlight;
        public Submission SelectedFlight
        {
            get { return _selectedFlight; }
            set 
            {
                var status = Set(ref _selectedFlight, value); 
            }
        }

        private Package _selectedPackage;
        public Package SelectedPackage
        {
            get { return _selectedPackage; }
            set { Set(ref _selectedPackage, value); }
        }

        private StoreService _storeService = SimpleIoc.Default.GetInstance<StoreService>();

        private SynchronizationContext _threadContext = SynchronizationContext.Current;


        public FlightsViewModel()
        {    
            _products = new ObservableCollection<Product>();
            _flights = new ObservableCollection<Submission>();
            _flightPackages = new ObservableCollection<Package>();
        }

        public async Task Init()
        {
            try
            {
                HomeViewModel.BusyVM.IsProcessing = true;

                await _storeService.PrepareCookie();

                FlightPackages.Clear();
                SelectedPackage = null;

                Flights.Clear();
                SelectedFlight = null;

                var products = await _storeService.GetProducts();
                Products.Clear();
                foreach (var item in products)
                {
                    Products.Add(item);
                }
            }
            catch(Exception e)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                new MessageDialog(e.Message).ShowAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            finally
            {
                HomeViewModel.BusyVM.IsProcessing = false;
            }
        }

        public async void GetFlgihts()
        {
            try
            {
                HomeViewModel.BusyVM.IsProcessing = true;
                FlightPackages.Clear();
                SelectedPackage = null;
                var flights = await _storeService.GetSubmissions(SelectedProduct.BigId);
                Flights.Clear();
                foreach (var item in flights)
                {
                    Flights.Add(item);
                }
            }
            catch(Exception e)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                new MessageDialog(e.Message).ShowAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            finally
            {
                HomeViewModel.BusyVM.IsProcessing = false;
            }           
        }

        public async void GetFlightPackages()
        {
            try
            {
                HomeViewModel.BusyVM.IsProcessing = true;
                var packages = await _storeService.GetPackages(SelectedProduct.BigId,SelectedFlight.Id);
                FlightPackages.Clear();
                foreach (var item in packages)
                {
                    FlightPackages.Add(item);
                }
            }
            catch(Exception e)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                new MessageDialog(e.Message).ShowAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            finally
            {
                HomeViewModel.BusyVM.IsProcessing = false;
            }
        }

        public async void RequestPreinstallKit(Package package)
        {
            try
            {
                HomeViewModel.BusyVM.IsProcessing = true;
                await _storeService.GeneratePreinstallKit(package.PackageId);
            }
            catch(Exception e)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                new MessageDialog(e.Message).ShowAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            finally
            {
                HomeViewModel.BusyVM.IsProcessing = false;
            }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                while (true)
                {
                    var workflow = await _storeService.QueryPreinstallKitWorkflowStatus(package.PackageId);
                    switch (workflow.WorkflowState)
                    {
                        case WorkflowState.WorkflowQueued:
                        case WorkflowState.GeneratePreinstallPackageInProgress:
                            _threadContext.Post((o) =>
                            {
                                package.PreinstallKitStatus = PreinstallKitStatus.Generating;
                            }, null);
                            break;
                        case WorkflowState.GeneratePreinstallPackageComplete:
                            _threadContext.Post((o) =>
                            {
                                package.PreinstallKitStatus = PreinstallKitStatus.Ready;
                                GetFlightPackages();
                            }, null);
                            return;
                        case WorkflowState.GeneratePreinstallPackageFailed:
                            _threadContext.Post((o) =>
                            {
                                package.PreinstallKitStatus = PreinstallKitStatus.NeedToGenerate;
                            }, null);
                            return;
                        default:
                            return;
                    }
                    await Task.Delay(500);
                }
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
