using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DevCenterGallary.Common.Models
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
}
