using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DevCenterGallery.Web.Models;
using DevCenterGalley.Common.Services;
using DevCenterGalley.Common.Models;
using DevCenterGallery.Web.Data;
using Microsoft.EntityFrameworkCore;
using DevCenterGallery.Common.Services;

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

            _cookieService = new CookieService();
            _storeService = new StoreService(_cookieService);
        }

        [HttpPost]
        public async Task<JsonResult> SyncDevCenter(string productId, string submissionId)
        {
            _logger.LogInformation($"Start to Sync DevCenter, ProductId: {productId}, SubmissionId: {submissionId}");
            string errorMsg = string.Empty;
            try
            {
                await _storeService.PrepareCookie();
                if (!string.IsNullOrEmpty(productId) && !string.IsNullOrEmpty(submissionId))
                {
                    await _syncPackages(productId, submissionId);
                }
                else if (!string.IsNullOrEmpty(productId))
                {
                    await _syncSubmissions(productId);
                }
                else
                {
                    await _syncProducts();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Sync failed. {e.Message}{Environment.NewLine}{e.StackTrace}");
                errorMsg = e.Message;
            }
            var result = new { result = string.IsNullOrEmpty(errorMsg) ? 200 : 400, msg = errorMsg };
            return Json(result);
        }

        private async Task<IList<Product>> _syncProducts()
        {
            var products = await _storeService.GetProductsAsync();
            var oldProducts = _dbContext.Products.ToList();
            foreach (var prod in products)
            {
                var oldProd = oldProducts.FirstOrDefault(m => m.BigId == prod.BigId);
                if (oldProd == null)
                {
                    oldProducts.Add(prod);
                }
                else
                {
                    oldProd.LogoUri = prod.LogoUri;
                    oldProd.Name = prod.Name;
                }
            }
            _dbContext.Products.UpdateRange(oldProducts);
            _dbContext.SaveChanges();
            return oldProducts;
        }

        private async Task<IList<Submission>> _syncSubmissions(string productId)
        {
            var submissions = await _storeService.GetSubmissionsAsync(productId);
            var product = _dbContext.Products.Include(m=>m.Submissions).FirstOrDefault(m => m.BigId == productId);
            var oldSubmissions = product.Submissions;
            foreach (var sub in submissions)
            {
                var oldSub = oldSubmissions.FirstOrDefault(m => m.SubmissionId == sub.SubmissionId);
                if (oldSub == null)
                {
                    sub.Product= product;
                    oldSubmissions.Add(sub);
                }
                else
                {
                    oldSub.FriendlyName = sub.FriendlyName;
                    oldSub.PublishedDateTime = sub.PublishedDateTime;
                    oldSub.ReleaseRank = sub.ReleaseRank;
                    oldSub.UpdatedDateTime = sub.UpdatedDateTime;
                }
            }
            _dbContext.Products.Update(product);
            _dbContext.SaveChanges();
            return oldSubmissions;
        }

        private async Task<IList<Package>> _syncPackages(string productId, string submissionId)
        {
            var packages = await _storeService.GetPackagesAsync(productId, submissionId);
            var submission = _dbContext.Submissions.Include(m=>m.Packages).FirstOrDefault(m => m.SubmissionId == submissionId);
            var oldPackages = submission.Packages;
            foreach (var package in packages)
            {
                var oldPackage = oldPackages.FirstOrDefault(m => m.PackageId == package.PackageId);
                if (oldPackage == null)
                {   
                    package.Submission = submission;
                    oldPackages.Add(package);
                }
                else
                {
                    oldPackage.Architecture = package.Architecture;
                    oldPackage.Assets = package.Assets;
                    oldPackage.FileName = package.FileName;
                    oldPackage.PackageVersion = package.PackageVersion;
                    oldPackage.RuntimeTargetPlatforms = package.RuntimeTargetPlatforms;
                    // Will be calculated when get package request.
                    //oldPackage.TargetPlatform = package.TargetPlatform;
                    //oldPackage.PackgeFileInfo = package.PackgeFileInfo;
                    //oldPackage.PreinstallKitStatus = package.PreinstallKitStatus;
                }
            }
            _dbContext.Submissions.Update(submission);
            _dbContext.SaveChanges();
            return packages;
        }

        public IActionResult Products()
        {
            return View("Products", _dbContext.Products.ToList());
        }

        public IActionResult Submissions(string productId)
        {
            var product = _dbContext.Products.Include(m => m.Submissions).Where(m => m.BigId == productId).FirstOrDefault();
            product.Submissions = product.Submissions.OrderByDescending(m => m.ReleaseRank).ToList();
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
