using DevCenterGallary.Common.Models;
using DevCenterGallary.Common.Services;
using DevCenterGallery.Web.Data;
using Pomelo.AspNetCore.TimedJob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevCenterGallery.Web.Jobs
{
    public class DevCenterJob:Job
    {
        private StoreService _storeService;
        private ICookieService _cookieService;
        private DevCenterContext _dbContext;

        public DevCenterJob()
        {
            _dbContext = new DevCenterContext();
            _cookieService = new PersonalCookieService();
            _storeService = new StoreService(_cookieService);
        }

        [Invoke(Begin = "2020.09.09 05:04:00", Interval = 1000 * 60 * 60 * 3, SkipWhileExecuting = true)]
        public async void RefreshProducts()
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
                        submission.Packages = await _storeService.GetPackagesAsync(product.BigId, submission.SubmissionId);
                    }
                }
                _dbContext.Products.RemoveRange(_dbContext.Products);
                _dbContext.Products.AddRange(products);
                _dbContext.SaveChanges();
            }
            catch
            {

            }
        }
    }
}
