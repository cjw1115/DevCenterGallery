using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using DevCenterGallary.Common.Models;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.IO;
using GalaSoft.MvvmLight.Ioc;

namespace DevCenterGallary.Common.Services
{
    public class StoreService
    {
        private static readonly string _hostUri = "https://partner.microsoft.com/";
        private string _productsUri = _hostUri + "en-us/dashboard/api/v2/products";

        private string _bigId;
        private string _submissonId;
        private string _packageId;
        private string _getSubmissionsUri => $"https://partner.microsoft.com/dashboard/packages/api/sub/api/products/{_bigId}/submissions";
        private string _getPackagesUri => $"https://partner.microsoft.com/dashboard/packages/api/pkg/v2.0/packagesets?productId={_bigId}&submissionId={_submissonId}&select=PackageAsset";

        private string _generatePreinstallKitUri => $"https://partner.microsoft.com/dashboard/packages/api/pkg/v2.0/packages/{_packageId}/assets/UAPPreinstalledBinary/generatePreinstall";
        private string _queryPreinstallKitWorkflowUri => $"https://partner.microsoft.com/dashboard/packages/api/pkg/v2.0/packages/{_packageId}/workflows";

        private List<string> _productFilter = new List<string>
        {
        };

        class ProductsModel
        {
            public IList<Product> productList { get; set; }
        }

        class PackagesModel
        {
            public IList<Package> Packages { get; set; }
        }

        private HttpService _httpService = new HttpService();
        private ICookieService _cookieService => SimpleIoc.Default.GetInstance<ICookieService>();
 
        public StoreService()
        {
        }

        public async Task PrepareCookie()
        {
            try
            {
                var cookie = await _cookieService.GetDevCenterCookie();
                _httpService.SetCookie(_hostUri, cookie);
            }
            catch
            {
                throw;
            }
            finally
            {
            }
        }

        public async Task<IList<Product>> GetProducts()
        {
            var stringContent = await _httpService.SendRequestAndGetString(_productsUri, Encoding.UTF8, HttpMethod.Get);
            var products = JsonSerializer.Deserialize<ProductsModel>(stringContent).productList.AsEnumerable();
            if (!(_cookieService is PersonalCookieService))
            {
                products = JsonSerializer.Deserialize<ProductsModel>(stringContent).productList.Where(m => _productFilter.Contains(m.BigId));
            }
            foreach (var item in products)
            {
                item.LogoUri = new Uri(new Uri(_hostUri), item.LogoUri).ToString();
                try
                {
                    using (var imgStream = await _httpService.SendRequestAndGetStream(item.LogoUri, HttpMethod.Get))
                    {
                        BitmapImage image = new BitmapImage();
                        await image.SetSourceAsync(imgStream.AsRandomAccessStream());
                        item.ImageSource = image;
                    }
                }
                catch
                {
                }
            }
            return products.ToList();
        }

        public async Task<IList<Submission>> GetSubmissions(string productId)
        {
            _bigId = productId;
            var stringContent = await _httpService.SendRequestAndGetString(_getSubmissionsUri, Encoding.UTF8, HttpMethod.Get);
            var submissions = JsonSerializer.Deserialize<IList<Submission>>(stringContent);
            return submissions.OrderByDescending(m => m.ReleaseRank).ToList();
        }

        public async Task<IList<Package>> GetPackages(string productId, string submissionId)
        {
            _bigId = productId;
            _submissonId = submissionId;
            var stringContent = await _httpService.SendRequestAndGetString(_getPackagesUri, Encoding.UTF8, HttpMethod.Get);
            var submissions = JsonSerializer.Deserialize<IList<PackagesModel>>(stringContent);
            if(submissions!=null&& submissions.Count>0)
            {
                foreach (var item in submissions[0].Packages)
                {
                    item.PcakgeFileInfo = item.Assets.FirstOrDefault(m => m.AssetType == "UAPPreinstalledBinary")?.FileInfo;
                    item.PreinstallKitStatus = item.PcakgeFileInfo != null ? PreinstallKitStatus.Ready : PreinstallKitStatus.NeedToGenerate;
                    item.TargetPlatform = item.RuntimeTargetPlatforms.First();
                }
                return submissions[0].Packages;
            }

            return null;
        }

        public async Task GeneratePreinstallKit(string packageId)
        {
            _packageId = packageId;
            await _httpService.SendRequestAndGetStream(_generatePreinstallKitUri,HttpMethod.Post);
        }

        public async Task<Workflow> QueryPreinstallKitWorkflowStatus(string packageId)
        {
            _packageId = packageId;
            var stringContent = await _httpService.SendRequestAndGetString(_queryPreinstallKitWorkflowUri, Encoding.UTF8, HttpMethod.Get);
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            var workflows = JsonSerializer.Deserialize<IList<Workflow>>(stringContent, options);
            if (workflows != null && workflows.Count > 0)
            {
                return workflows[0];
            }
            return null;
        }

        #region Customer Group
        public async Task<CustomerGroup> GetGroupInfo(string groupId)
        {
            var uri = $"{_hostUri}dashboard/monetization/group-management/api/groups/{groupId}?members=true";
            var strContent = await _httpService.SendRequestAndGetString(uri, Encoding.UTF8, HttpMethod.Get);
            return JsonSerializer.Deserialize<CustomerGroup>(strContent);
        }

        public async Task<IList<CustomerGroup>> GetGroups()
        {
            var uri = $"{_hostUri}dashboard/monetization/group-management/api/groups";
            var strContent = await _httpService.SendRequestAndGetString(uri, Encoding.UTF8, HttpMethod.Get);
            return JsonSerializer.Deserialize<IList<CustomerGroup>>(strContent);
        }

        public async Task<CustomerGroup> UpdateGroup(string groupId, UpdateCustomerGroup group)
        {
            string uri = $"{_hostUri}dashboard/monetization/group-management/api/groups/{groupId}";

            StringContent content = new StringContent(JsonSerializer.Serialize(group));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var strContent = await _httpService.SendRequestAndGetString(uri, Encoding.UTF8, HttpMethod.Put, content);
            return JsonSerializer.Deserialize<CustomerGroup>(strContent);
        }
        #endregion
    }
}
