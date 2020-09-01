using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DevCenterGallery.Web.Models;
using DevCenterGallary.Common.Services;
using DevCenterGallary.Common.Models;
using System.IO;

namespace DevCenterGallery.Web.Controllers
{
    public class DevCenterController : Controller
    {
        private readonly ILogger<DevCenterController> _logger;
        private StoreService _storeService;
        private ICookieService _cookieService;

        public DevCenterController(ILogger<DevCenterController> logger)
        {
            _logger = logger;
            _cookieService = new PersonalCookieService();
            _storeService = new StoreService(_cookieService);
        }

        private Stream _getProductFile()
        {
            var productsDirStr = Path.Combine(Directory.GetCurrentDirectory(), "Products");
            var productsDir = Directory.CreateDirectory(productsDirStr);
            if (!productsDir.Exists)
            {
                productsDir.Create();
            }
            var productFileStr = Path.Combine(productsDirStr, "Products.json");
            return System.IO.File.Open(productFileStr, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public async Task<IActionResult> Products()
        {
            List<Product> productList = new List<Product>();
            try
            {
                string productsStr;
                var productsFileStream = _getProductFile();
                StreamReader reader = new StreamReader(productsFileStream);
                productsStr = reader.ReadToEnd();
                try
                {
                    productList.AddRange(System.Text.Json.JsonSerializer.Deserialize<IList<Product>>(productsStr));
                }
                catch
                {

                }

                if (productList.Count <= 0)
                {
                    await _storeService.PrepareCookie();
                    var products = await _storeService.GetProducts();
                    productList.AddRange(products);

                    productsStr = System.Text.Json.JsonSerializer.Serialize(products);

                    productsFileStream.Seek(0, SeekOrigin.Begin);
                    using (StreamWriter writer = new StreamWriter(productsFileStream))
                    {
                        writer.Write(productsStr);
                    }
                }
                productsFileStream.Close();
            }
            catch
            {

            }
            return View("Flights", productList);
        }

        public async Task<IActionResult> Flights(Product product)
        {
            var submissions = await _storeService.GetSubmissions(product.BigId);
            return View(submissions);
        }

        public async Task<IActionResult> Packages(string productId,string submissionId)
        {
            var pacakges = await _storeService.GetPackages(productId, submissionId);
            return View(pacakges);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

//        public async void RequestPreinstallKit(Package package)
//        {
//            try
//            {
//                HomeViewModel.BusyVM.IsProcessing = true;
//                await _storeService.GeneratePreinstallKit(package.PackageId);
//            }
//            catch (Exception e)
//            {
//#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
//                new MessageDialog(e.Message).ShowAsync();
//#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
//            }
//            finally
//            {
//                HomeViewModel.BusyVM.IsProcessing = false;
//            }

//#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
//            Task.Run(async () =>
//            {
//                while (true)
//                {
//                    var workflow = await _storeService.QueryPreinstallKitWorkflowStatus(package.PackageId);
//                    switch (workflow.WorkflowState)
//                    {
//                        case WorkflowState.WorkflowQueued:
//                        case WorkflowState.GeneratePreinstallPackageInProgress:
//                            _threadContext.Post((o) =>
//                            {
//                                package.PreinstallKitStatus = PreinstallKitStatus.Generating;
//                            }, null);
//                            break;
//                        case WorkflowState.GeneratePreinstallPackageComplete:
//                            _threadContext.Post((o) =>
//                            {
//                                package.PreinstallKitStatus = PreinstallKitStatus.Ready;
//                                GetFlightPackages();
//                            }, null);
//                            return;
//                        case WorkflowState.GeneratePreinstallPackageFailed:
//                            _threadContext.Post((o) =>
//                            {
//                                package.PreinstallKitStatus = PreinstallKitStatus.NeedToGenerate;
//                            }, null);
//                            return;
//                        default:
//                            return;
//                    }
//                    await Task.Delay(500);
//                }
//            });
//#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
//        }
    }
}
