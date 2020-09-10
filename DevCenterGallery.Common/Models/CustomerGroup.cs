using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DevCenterGalley.Common.Models
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
