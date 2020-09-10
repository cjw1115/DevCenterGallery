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
using DevCenterGallery.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace DevCenterGallery.Web.Controllers
{
    public class DevCenterController : Controller
    {
        private readonly ILogger<DevCenterController> _logger;
        private StoreService _storeService;
        private ICookieService _cookieService;
        private DevCenterContext _dbContext;

        public DevCenterController(ILogger<DevCenterController> logger, DevCenterContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;

            _cookieService = new PersonalCookieService();
            _storeService = new StoreService(_cookieService);
        }

        [HttpPost]
        public async Task<JsonResult> SyncDevCenter(string productId, string submissionsId, string packageId)
        {
            string errorMsg = string.Empty;
            try
            {
                await _storeService.PrepareCookie();
                var products = await _storeService.GetProductsFullInfoAsync();
                var oldProducts = _dbContext.Products.ToList();
                _dbContext.RemoveRange(oldProducts);
                _dbContext.SaveChanges();
                _dbContext.Products.AddRange(products);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                errorMsg = e.Message;
            }
            var result = new { result = string.IsNullOrEmpty(errorMsg) ? 200 : 400, msg = errorMsg };
            return Json(result);
        }
        public IActionResult Products()
        {
            return View("Products", _dbContext.Products.ToList());
        }

        public IActionResult Submissions(string productId)
        {
            var product = _dbContext.Products.Include(m => m.Submissions).Where(m => m.BigId == productId).FirstOrDefault();
            ViewData["productId"] = productId;
            return View("Submissions", product);
        }

        public IActionResult Packages(string productId, string submissionId)
        {
            var product = _dbContext.Products.Where(m => m.BigId == productId).FirstOrDefault();
            ViewData["productId"] = productId;
            ViewData["productName"] = product.Name;
            ViewData["submissionId"] = submissionId;

            var submission = _dbContext.Submissions.Include(m => m.Packages).FirstOrDefault(m => m.SubmissionId == submissionId);
            var pacakges = new List<Package>();
            if (submission != null)
            {
                foreach (var item in submission.Packages)
                {
                    var pack = _dbContext.Packages
                        .Include(m => m.Assets).ThenInclude(m => m.FileInfo)
                        .Include(m => m.RuntimeTargetPlatforms)
                        .Where(m => m.PackageId == item.PackageId).FirstOrDefault();


                    pack.PackgeFileInfo = item.Assets?.FirstOrDefault(m => m.AssetType == "UAPPreinstalledBinary")?.FileInfo;
                    pack.PreinstallKitStatus = item.PackgeFileInfo != null ? PreinstallKitStatus.Ready : PreinstallKitStatus.NeedToGenerate;
                    pack.TargetPlatform = item.RuntimeTargetPlatforms.First();

                    if (pacakges != null)
                    {
                        pacakges.Add(pack);
                    }
                }
                submission.Packages = pacakges;
            }
            return View("Packages", submission);
        }

        [HttpPost]
        public async Task<JsonResult> GeneratePreinstallKit(string packageId)
        {
            await _storeService.PrepareCookie();
            await _storeService.GeneratePreinstallKit(packageId);
            return Json(new { status = PreinstallKitStatus.Generating.ToString() });
        }

        public async Task<JsonResult> QueryPreinstallKitWorkflowStatus(string productId, string submissionId, string packageId)
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

                    var package = await _storeService.GetPackagesAsync(productId, submissionId);
                    var submission = _dbContext.Submissions.Include(m => m.Packages).FirstOrDefault(m => m.SubmissionId == submissionId);
                    submission.Packages.Clear();
                    foreach (var item in package)
                    {
                        submission.Packages.Add(item);
                    }
                    _dbContext.SaveChanges();

                    break;
                case WorkflowState.GeneratePreinstallPackageFailed:
                    preinstallKitStatus = PreinstallKitStatus.NeedToGenerate;
                    break;
                default:
                    break;
            }
            return Json(new { status = preinstallKitStatus.ToString() });
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

        [HttpPost]
        public IActionResult Clean()
        {
            var products = _dbContext.Products.ToList();
            _dbContext.RemoveRange(products);
            _dbContext.SaveChanges();
            return RedirectToAction("Products");
        }
    }
}
