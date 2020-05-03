using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SignInDevCenter
{
    public class CustomerGroup
    {
        [JsonPropertyName("id")]
        public string GroupId { get; set; }

        [JsonPropertyName("name")]
        public string GroupName { get; set; }

        [JsonPropertyName("members")]
        public IList<string> Members { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime LastUpdatedTime { get; set; }
    }

    public class UpdateCustomerGroup
    {
        public List<string> Members { get; set; } = new List<string>();
        public string Name { get; set; }
        public string Type { get; set; } = "User";
    }

    public class DevCenterService
    {
        private HttpService _httpService;

        private readonly string _devCenterUri = "https://partner.microsoft.com/";

        public DevCenterService(string cookieStr)
        {
            _httpService = new HttpService(new Uri(_devCenterUri), cookieStr);
        }

        public async Task<CustomerGroup> GetGroupInfo(string groupId)
        {
            var uri = $"{_devCenterUri}dashboard/monetization/group-management/api/groups/{groupId}?members=true";
            var strContent = await _httpService.SendRequestForString(uri, HttpMethod.Get);
            return JsonSerializer.Deserialize<CustomerGroup>(strContent);
        }

        public async Task<IList<CustomerGroup>> GetGroups()
        {
            var uri = $"{_devCenterUri}dashboard/monetization/group-management/api/groups";
            var strContent = await _httpService.SendRequestForString(uri, HttpMethod.Get);
            return JsonSerializer.Deserialize<IList<CustomerGroup>>(strContent);
        }

        public async Task<CustomerGroup> UpdateGroup(string groupId, UpdateCustomerGroup group)
        {
            string uri = $"{_devCenterUri}dashboard/monetization/group-management/api/groups/{groupId}";

            StringContent content = new StringContent(JsonSerializer.Serialize(group));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var strContent = await _httpService.SendRequestForString(uri, HttpMethod.Put, content);
            return JsonSerializer.Deserialize<CustomerGroup>(strContent);
        }
    }
}
