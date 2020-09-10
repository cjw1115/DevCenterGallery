using DevCenterGalley.Common.Services;
using DevCenterGallery.Common.Services;
using DevCenterGallery.Web.Data;
using Pomelo.AspNetCore.TimedJob;
using System.Linq;

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
            _cookieService = new CookieService();
            _storeService = new StoreService(_cookieService);
        }

        [Invoke(Begin = "2020.09.10 12:28:00", Interval = 1000 * 60 * 60 * 3, SkipWhileExecuting = true)]
        public async void RefreshProducts()
        {
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
            catch
            {
            }
        }
    }
}
