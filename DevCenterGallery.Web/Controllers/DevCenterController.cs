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
using System.Threading;

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

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            //_refreshProductsJob();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            _prepareData();
        }

        private List<Product> _products;
        private void _prepareData()
        {
            _products = new List<Product>();
            try
            {
                string productsStr;
                var productsFileStream = _getProductFile();
                StreamReader reader = new StreamReader(productsFileStream);
                productsStr = reader.ReadToEnd();
                try
                {
                    _products.AddRange(System.Text.Json.JsonSerializer.Deserialize<IList<Product>>(productsStr));
                }
                catch
                {

                }
                productsFileStream.Close();
            }
            catch
            {
            }
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

        public IActionResult Products()
        {
            return View("Products", _products);
        }

        public IActionResult Submissions(string productId)
        {
            var product = _products.FirstOrDefault(m => m.BigId == productId);
            ViewData["productId"] = productId;
            return View("Submissions", product);
        }

        public IActionResult Packages(string productId, string submissionId)
        {
            var product = _products.FirstOrDefault(m => m.BigId == productId);
            var submission = product.Submissions.FirstOrDefault(m => m.Id == submissionId);
            ViewData["productId"] = productId;
            ViewData["submissionId"] = submissionId;
            ViewData["productName"] = product.Name;
            return View("Packages", submission);
        }

        [HttpPost]
        public async Task<JsonResult> GeneratePreinstallKit(string packageId)
        {
            await _storeService.PrepareCookie();
            await _storeService.GeneratePreinstallKit(packageId);
            return Json(new { status = PreinstallKitStatus.Generating.ToString() });
        }

        public async Task<JsonResult> QueryPreinstallKitWorkflowStatus(string packageId)
        {
            await _storeService.PrepareCookie();
            var workflow = await _storeService.QueryPreinstallKitWorkflowStatus(packageId);
            var preinstallKitStatus = PreinstallKitStatus.NeedToGenerate;
            switch (workflow.WorkflowState)
            {
                case WorkflowState.WorkflowQueued:
                case WorkflowState.GeneratePreinstallPackageInProgress:
                    preinstallKitStatus = PreinstallKitStatus.Generating;
                    break;
                case WorkflowState.GeneratePreinstallPackageComplete:
                    preinstallKitStatus = PreinstallKitStatus.Ready;
                    break;
                case WorkflowState.GeneratePreinstallPackageFailed:
                    preinstallKitStatus = PreinstallKitStatus.NeedToGenerate;
                    break;
                default:
                    break;
            }
            return Json(new { status = preinstallKitStatus.ToString()});
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

        private async Task _refreshProductsJob()
        {
            try
            {
                await _storeService.PrepareCookie();
                var products = await _storeService.GetProductsAsync();
                foreach (var product in products)
                {
                    product.Submissions = await _storeService.GetSubmissionsAsync(product.BigId);
                    foreach (var submission in product.Submissions)
                    {
                        submission.Packages = await _storeService.GetPackagesAsync(product.BigId, submission.Id);
                    }
                }
                var productsStr = System.Text.Json.JsonSerializer.Serialize(products);
                using (StreamWriter writer = new StreamWriter(_getProductFile()))
                {
                    writer.Write(productsStr);
                }
            }
            catch
            {

            }
            
        }
    }
}
