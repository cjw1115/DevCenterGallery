﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using DevCenterGalley.Common.Models;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text;
using System.IO;

namespace DevCenterGalley.Common.Services
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

        public List<string> ProductFilter 
        {
            get
            {
                List<string> products = new List<string>();
                using (var configStream = File.Open(Path.Combine(Directory.GetCurrentDirectory(), "DevCenterConfig.json"), FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[configStream.Length];
                    configStream.Read(buffer, 0, buffer.Length);
                    var configJson = JsonDocument.Parse(Encoding.UTF8.GetString(buffer));
                    string usernmae = string.Empty;
                    string password = string.Empty;

                    JsonElement productFilters = new JsonElement();
                    foreach (var item in configJson.RootElement.EnumerateObject())
                    {
                        if (item.Name == "ProductFilter")
                        {
                            productFilters = item.Value;
                        }
                    }
                    foreach (var item in productFilters.EnumerateArray())
                    {
                        products.Add(item.GetString());
                    }
                    return products;
                }
            }
        }

        class ProductsModel
        {
            public List<Product> productList { get; set; }
        }

        class PackagesModel
        {
            public List<Package> Packages { get; set; }
        }

        private HttpService _httpService = new HttpService();
        private ICookieService _cookieService;
 
        public StoreService(ICookieService cookieService)
        {
            _cookieService = cookieService;
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

        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                var stringContent = await _httpService.SendRequestAndGetString(_productsUri, Encoding.UTF8, HttpMethod.Get);
                var products = JsonSerializer.Deserialize<ProductsModel>(stringContent).productList.AsEnumerable();
                if (!(_cookieService is PersonalCookieService))
                {
                    var filters = ProductFilter;
                    if(filters!=null&& filters.Count > 0)
                    {
                        products = JsonSerializer.Deserialize<ProductsModel>(stringContent).productList.Where(m => filters.Contains(m.BigId));
                    }
                    else
                    {
                        products = JsonSerializer.Deserialize<ProductsModel>(stringContent).productList;
                    }
                }
                foreach (var item in products)
                {
                    item.LogoUri = new Uri(new Uri(_hostUri), item.LogoUri).ToString();
                }
                return products.ToList();
            }
            catch 
            {
            }
            return new List<Product>(0);
        }

        public async Task<List<Submission>> GetSubmissionsAsync(string productId)
        {
            try
            {
                _bigId = productId;
                var stringContent = await _httpService.SendRequestAndGetString(_getSubmissionsUri, Encoding.UTF8, HttpMethod.Get);
                var submissions = JsonSerializer.Deserialize<List<Submission>>(stringContent);
                return submissions.OrderByDescending(m => m.ReleaseRank).ToList();
            }
            catch
            {
            }
            return new List<Submission>(0);
        }

        public async Task<List<Package>> GetPackagesAsync(string productId, string submissionId)
        {
            try
            {
                _bigId = productId;
                _submissonId = submissionId;
                var stringContent = await _httpService.SendRequestAndGetString(_getPackagesUri, Encoding.UTF8, HttpMethod.Get);
                var submissions = JsonSerializer.Deserialize<List<PackagesModel>>(stringContent);
                if (submissions != null && submissions.Count > 0)
                {
                    return submissions[0].Packages;
                }
            }
            catch
            {
            }
            return new List<Package>(0);
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
            var workflows = JsonSerializer.Deserialize<List<Workflow>>(stringContent, options);
            if (workflows != null && workflows.Count > 0)
            {
                return workflows[0];
            }
            return null;
        }

        public async Task<List<Product>> GetProductsFullInfoAsync()
        {
            var products = await GetProductsAsync();
            foreach (var product in products)
            {
                product.Submissions = await GetSubmissionsAsync(product.BigId);
                foreach (var submission in product.Submissions)
                {
                    submission.Product = product;
                    submission.Packages = await GetPackagesAsync(product.BigId, submission.SubmissionId);
                    foreach (var package in submission.Packages)
                    {
                        package.Submission = submission;
                    }
                }
            }
            return products;
        }

        #region Customer Group
        public async Task<CustomerGroup> GetGroupInfo(string groupId)
        {
            var uri = $"{_hostUri}dashboard/monetization/group-management/api/groups/{groupId}?members=true";
            var strContent = await _httpService.SendRequestAndGetString(uri, Encoding.UTF8, HttpMethod.Get);
            return JsonSerializer.Deserialize<CustomerGroup>(strContent);
        }

        public async Task<List<CustomerGroup>> GetGroups()
        {
            var uri = $"{_hostUri}dashboard/monetization/group-management/api/groups";
            var strContent = await _httpService.SendRequestAndGetString(uri, Encoding.UTF8, HttpMethod.Get);
            return JsonSerializer.Deserialize<List<CustomerGroup>>(strContent);
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
